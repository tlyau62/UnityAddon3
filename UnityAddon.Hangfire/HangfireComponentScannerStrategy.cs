using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Hangfire
{
    public class HangfireComponentScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinition Create(Type type)
        {
            return new SimpleFactoryBeanDefinition(type, (c, t, n) =>
            {
                var proxyGenerator = c.ResolveUA<ProxyGenerator>();
                var hangfireProxyInterceptor = c.ResolveUA<HangfireProxyInterceptor>();

                return proxyGenerator.CreateInterfaceProxyWithoutTarget(t, hangfireProxyInterceptor);
            });
        }

        public bool IsMatch(Type type)
        {
            return type.IsInterface && type.HasAttribute<HangfireProxyAttribute>();
        }
    }
}
