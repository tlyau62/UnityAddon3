using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Hangfire
{
    public class HangfireComponentScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinitionCollection Create(Type type)
        {
            var qualifiers = type.HasAttribute<QualifierAttribute>() ? type.GetAttribute<QualifierAttribute>().Values : new string[0];
            var proxybeanDef = new SimpleFactoryBeanDefinition(type, $"{type.Name}HangfireProxy", ProxybeanFactory, qualifiers);

            return new BeanDefinitionCollection() { proxybeanDef };

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
