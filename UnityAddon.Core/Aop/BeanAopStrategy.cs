using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Handle the aop proxy on a bean.
    /// There are 2 kinds aop proxy: method and class proxy.
    /// 
    /// if a bean contains any method marked with method interceptor attribute,
    /// the bean will be proxied with a interceptor "AopMethodBootstrapInterceptor".
    /// 
    /// if a bean type is marked with a class interceptor attribute,
    /// the bean will be proxied with that class attribute interceptor.
    /// </summary>
    [Component]
    public class BeanAopStrategy : BuilderStrategy
    {
        [Dependency]
        public AopMethodBootstrapInterceptor AopInterceptor { get; set; }

        [Dependency]
        public InterfaceProxyFactory InterfaceProxyFactory { get; set; }

        [Dependency]
        public AopInterceptorContainer AopInterceptorContainer { get; set; }

        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            if (context.Existing == null)
            {
                base.PostBuildUp(ref context);
                return;
            }

            var interceptors = new List<IInterceptor>();
            var types = GetUnproxiedTypes(context.Existing);

            // class interceptor
            var typeInterceptorsMap = AopInterceptorContainer.FindInterceptors(AttributeTargets.Class | AttributeTargets.Interface);

            foreach (var type in types)
            {
                foreach (var attribute in type.GetCustomAttributes(false))
                {
                    if (typeInterceptorsMap.ContainsKey(attribute.GetType()))
                    {
                        interceptors.AddRange(typeInterceptorsMap[attribute.GetType()]);
                    }
                }
            }

            // method interceptor
            if (IsMethodBootstrapInterceptorNeeded(types))
            {
                interceptors.Add(AopInterceptor);
            }

            if (interceptors.Count() > 0)
            {
                if (ProxyUtil.IsProxy(context.Existing))
                {
                    AddInterceptorsToProxy(context.Existing, interceptors);
                }
                else
                {
                    context.Existing = InterfaceProxyFactory.CreateInterfaceProxy(context.Existing, interceptors.ToArray());
                }
            }

            if (ProxyUtil.IsProxy(context.Existing) && !HasInterceptors(context.Existing))
            {
                throw new InvalidOperationException($"No interceptor found for this proxy bean {context.Existing}.");
            }

            base.PostBuildUp(ref context);
        }

        private bool IsMethodBootstrapInterceptorNeeded(IEnumerable<Type> types)
        {
            var methodInterceptorsMap = AopInterceptorContainer.FindInterceptors(AttributeTargets.Method);

            return types.SelectMany(type => MethodSelector.GetAllMethods(type))
                .SelectMany(m => m.GetCustomAttributes())
                .Select(attr => attr.GetType())
                .Any(attrType => methodInterceptorsMap.ContainsKey(attrType));
        }

        /// <summary>
        /// Use with caution.
        /// Should be used only when the bean is not resolved or during resolving.
        /// Once the bean is resolved, adding interceptors to it is dangerous.
        /// </summary>
        private static void AddInterceptorsToProxy(object proxy, IEnumerable<IInterceptor> interceptors)
        {
            var field = (FieldInfo)proxy.GetType().GetMember("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(0);
            var proxyInterceptors = (IInterceptor[])field.GetValue(proxy);

            field.SetValue(proxy, proxyInterceptors.ToList().Union(interceptors).ToArray());
        }

        private static bool HasInterceptors(object proxy)
        {
            var field = (FieldInfo)proxy.GetType().GetMember("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(0);
            var proxyInterceptors = (IInterceptor[])field.GetValue(proxy);

            return proxyInterceptors.Length > 0;
        }

        private IEnumerable<Type> GetUnproxiedTypes(object instance)
        {
            var type = ProxyUtil.GetUnproxiedType(instance);

            if (ProxyUtil.IsProxyType(type))
            {
                return type.GetInterfaces();
            }

            return new[] { type };
        }

    }
}
