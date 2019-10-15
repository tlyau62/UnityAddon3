using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Aop;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Exceptions;
using UnityAddon.Reflection;

namespace UnityAddon.BeanBuildStrategies
{
    [Component]
    public class BeanInterceptionStrategy : BuilderStrategy
    {
        [Dependency]
        public AopInterceptor AopInterceptor { get; set; }

        [Dependency]
        public InterfaceProxyFactory InterfaceProxyFactory { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public ContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public InterceptorContainer InterceptorContainer { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            var interceptors = new List<IInterceptor>();

            var methodInterceptorsMap = InterceptorContainer.GetMethodInterceptorsDictionary();
            var a = MethodSelector.GetAllMethods(context.Type)
                .SelectMany(m => m.GetCustomAttributes());
            var isMethodAopNeeded = MethodSelector.GetAllMethods(context.Type)
                .SelectMany(m => m.GetCustomAttributes())
                .Select(attr => attr.GetType())
                .Any(attrType => methodInterceptorsMap.ContainsKey(attrType));

            // method interceptor
            if (isMethodAopNeeded)
            {
                interceptors.Add(AopInterceptor);
            }

            // class interceptor
            var classInterceptorsMap = InterceptorContainer.GetClassInterceptorsDictionary();

            foreach (var attribute in context.Type.GetCustomAttributes())
            {
                if (classInterceptorsMap.ContainsKey(attribute.GetType()))
                {
                    interceptors.AddRange(classInterceptorsMap[attribute.GetType()]);
                }
            }

            if (interceptors.Count() > 0)
            {
                context.Existing = InterfaceProxyFactory.CreateInterfaceProxy(context.Existing, interceptors.ToArray());
            }

            base.PostBuildUp(ref context);
        }

    }
}
