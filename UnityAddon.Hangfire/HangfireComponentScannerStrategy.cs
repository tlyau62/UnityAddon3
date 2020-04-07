using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Hangfire
{
    public class HangfireComponentScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinition Create(Type type)
        {
            var ProxyGenerator = new ProxyGenerator();
            var HangfireProxyInterceptor = new HangfireProxyInterceptor();

            return new SimpleFactoryBeanDefinition(type, (c, t, n) =>
            {
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(t, HangfireProxyInterceptor);
            });
        }

        public bool IsMatch(Type type)
        {
            return type.IsInterface && type.HasAttribute<HangfireProxyAttribute>();
        }
    }
}
