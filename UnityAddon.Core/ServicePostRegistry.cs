using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;

namespace UnityAddon.Core
{
    public interface IServicePostRegistry
    {
        void Add(IBeanDefinition beanDefinition);
        void AddSingleton(Type type, Type implType, string name);
        void AddSingleton(Type type, object instance, string name);
        void AddSingleton(Type type, Func<IServiceProvider, Type, string, object> factory, string name);
        void AddTransient(Type type, Type implType, string name);
        void AddTransient(Type type, Func<IServiceProvider, Type, string, object> factory, string name);
    }

    public class ServicePostRegistry : IServicePostRegistry
    {
        [Dependency]
        public ContainerBuilder ContainerBuilder { get; set; }

        public void Add(IBeanDefinition beanDefinition)
        {
            var entry = new ContainerBuilderEntry(ContainerBuilderEntryOrder.App, false);

            entry.ConfigureBeanDefinitions(config =>
            {
                config.Add(beanDefinition);
            });

            ContainerBuilder.Add(entry);

            ContainerBuilder.Refresh();
        }

        public void AddSingleton(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Singleton));
        }

        public void AddSingleton(Type type, object instance, string name)
        {
            Add(new InstanceBeanDefintion(type, instance, name, ScopeType.Singleton));
        }

        public void AddSingleton(Type type, Func<IServiceProvider, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Singleton));
        }

        public void AddTransient(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Transient));
        }

        public void AddTransient(Type type, Func<IServiceProvider, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Transient));
        }
    }
}
