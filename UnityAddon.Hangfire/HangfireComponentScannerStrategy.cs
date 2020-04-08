using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Hangfire
{
    public class HangfireComponentScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinitionCollection Create(Type type)
        {
            var beanDef = new TypeBeanDefinition(type);
            var proxybeanDef = new SimpleFactoryBeanDefinition(type, $"{type.Name}HangfireProxy", ProxybeanFactory);

            return new BeanDefinitionCollection() { beanDef, proxybeanDef };

            object ProxybeanFactory(IUnityContainer container, Type type, string name)
            {
                var proxyGenerator = container.ResolveUA<ProxyGenerator>();
                var hangfireProxyInterceptor = container.ResolveUA<HangfireProxyInterceptor>();

                return proxyGenerator.CreateInterfaceProxyWithoutTarget(type, hangfireProxyInterceptor);
            }
        }

        public bool IsMatch(Type type)
        {
            return type.IsInterface && type.HasAttribute<HangfireProxyAttribute>();
        }
    }
}
