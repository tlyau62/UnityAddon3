using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Aop
{
    /// <summary>
    /// The first interceptor triggered when a method of an aop proxied bean is invoked.
    /// This interceptor will try to trigger all the aop interceptors marked on a method.
    /// This is done by wrapping all the aop interceptors on that method into a new InterfaceProxy and
    /// invoke that intercepted method in this new proxy.
    /// </summary>
    /// <example>
    /// [Component]
    /// class AopProxiedService {
    ///     [InterceptorAttribute1] // InterceptorAttribute1 is invoked within AopMethodBootstrapInterceptor
    ///     [InterceptorAttribute2] // InterceptorAttribute1 is invoked within AopMethodBootstrapInterceptor
    ///     public void InterceptedMethod() { // when invoked, it executes AopMethodBootstrapInterceptor
    ///     }
    /// }
    /// </example>
    [Component]
    public class AopMethodBootstrapInterceptor : IInterceptor
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public InterfaceProxyFactory InterfaceProxyFactory { get; set; }

        [Dependency]
        public AopInterceptorContainer InterceptorContainer { get; set; }

        public void Intercept(IInvocation invocation)
        {
            object newProxy;
            MethodInfo proxyMethod;

            var aopInterceptors = GetMethodInterceptors(invocation.MethodInvocationTarget);
            var remainingInterceptors = GetRemainingInterceptors(invocation.Proxy);
            var mergeInterceptors = aopInterceptors.Union(remainingInterceptors);

            newProxy = InterfaceProxyFactory.CreateInterfaceProxy(invocation.InvocationTarget, mergeInterceptors.ToArray());
            proxyMethod = invocation.Method; // same interface method to the new proxy

            try
            {
                invocation.ReturnValue = proxyMethod.Invoke(newProxy, invocation.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw ex;
            }
        }

        private IEnumerable<IInterceptor> GetMethodInterceptors(MethodInfo method)
        {
            var interceptors = new List<IInterceptor>();
            var methodInterceptorMap = InterceptorContainer.FindInterceptors(AttributeTargets.Method);

            foreach (var attr in method.GetCustomAttributes())
            {
                if (methodInterceptorMap.ContainsKey(attr.GetType()))
                {
                    interceptors.AddRange(methodInterceptorMap[attr.GetType()]);
                }
            }

            return interceptors;
        }

        /// <summary>
        /// Get all interceptors after the AopInterceptor from the proxy object.
        /// </summary>
        private IInterceptor[] GetRemainingInterceptors(object proxy)
        {
            var field = (FieldInfo)proxy.GetType().GetMember("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(0);
            var interceptors = (IInterceptor[])field.GetValue(proxy);
            var found = false;
            var extracts = new List<IInterceptor>();

            foreach (var interceptor in interceptors)
            {
                if (found)
                {
                    extracts.Add(interceptor);
                }
                else if (interceptor.GetType() == typeof(AopMethodBootstrapInterceptor))
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new InvalidOperationException("AopIntercetor is not found.");
            }

            return extracts.ToArray();
        }

    }
}
