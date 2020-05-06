using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using System;
using Unity;
using System.Linq;
using UnityAddon.Core.Bean;
using Microsoft.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using UnityAddon.Core.Bean.DependencyInjection;

namespace UnityAddon.Core.BeanDefinition.MemberBean
{
    public class MemberConfigurationBeanDefinition : MemberComponentBeanDefinition
    {
        public MemberConfigurationBeanDefinition(Type type) : base(type)
        {
            if (type.HasAttribute<ConfigurationAttribute>() && !type.IsPublic)
            {
                throw new InvalidOperationException($"Configuration {type} must be public.");
            }
        }

        public override object Constructor(IServiceProvider serviceProvider, Type type, string name)
        {
            return serviceProvider.GetRequiredService<BeanFactory>()
                .ConstructClassProxy(type, new IInterceptor[] { serviceProvider.GetRequiredService<BeanMethodInterceptor>() }, serviceProvider);
        }
    }
}
