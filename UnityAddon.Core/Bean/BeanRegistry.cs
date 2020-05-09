using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;

namespace UnityAddon.Core.Bean
{
    public interface IBeanRegistry
    {
        void Add(IBeanDefinition beanDefinition);
        void AddSingleton(Type type, Type implType, string name);
        void AddSingleton<TMap, TImpl>(string name = null);
        void AddSingleton(Type type, object instance, string name);
        void AddSingleton<TMap>(TMap instance, string name = null);
        void AddSingleton(Type type, Func<IServiceProvider, Type, string, object> factory, string name);
        void AddSingleton<TMap>(Func<IServiceProvider, Type, string, TMap> factory, string name = null);
        void AddTransient(Type type, Type implType, string name);
        void AddTransient<TMap, TImpl>(string name = null);
        void AddTransient(Type type, Func<IServiceProvider, Type, string, object> factory, string name);
        void AddTransient<TMap>(Func<IServiceProvider, Type, string, TMap> factory, string name = null);
    }

    public abstract class BeanRegistry : IBeanRegistry
    {
        public abstract void Add(IBeanDefinition beanDefinition);

        public void AddSingleton(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap, TImpl>(string name = null)
        {
            AddSingleton(typeof(TMap), typeof(TImpl), name);
        }

        public void AddSingleton(Type type, object instance, string name)
        {
            Add(new InstanceBeanDefintion(type, instance, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap>(TMap instance, string name = null)
        {
            AddSingleton(typeof(TMap), instance, name);
        }

        public void AddSingleton(Type type, Func<IServiceProvider, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap>(Func<IServiceProvider, Type, string, TMap> factory, string name = null)
        {
            AddSingleton(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }

        public void AddTransient(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Transient));
        }

        public void AddTransient<TMap, TImpl>(string name = null)
        {
            AddTransient(typeof(TMap), typeof(TImpl), name);
        }

        public void AddTransient(Type type, Func<IServiceProvider, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Transient));
        }

        public void AddTransient<TMap>(Func<IServiceProvider, Type, string, TMap> factory, string name = null)
        {
            AddTransient(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }
    }
}
