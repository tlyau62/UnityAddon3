using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Exceptions;

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
        void Unregister(Type type, string name);
    }

    public class ServicePostRegistry : IServicePostRegistry
    {
        [Dependency]
        public ApplicationContext AppContext { get; set; }

        [Dependency]
        public IServiceProvider Sp { get; set; }

        public void Add(IBeanDefinition beanDefinition)
        {
            var entry = new ApplicationContextEntry(ApplicationContextEntryOrder.App, false);

            entry.ConfigureBeanDefinitions(config =>
            {
                config.Add(beanDefinition);
            });

            AppContext.AddContextEntry(entry);

            AppContext.Refresh();
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

        public void Unregister(Type type, string name)
        {
            var container = ((IUnityServiceProvider)Sp).UnityContainer;
            var beanDefContainer = Sp.GetRequiredService<IBeanDefinitionContainer>();
            var bean = Sp.GetRequiredService(type, name);
            var beanDef = beanDefContainer.RemoveBeanDefinition(type, name);
            var matchedList = container.Registrations.Where(p => p.RegisteredType == beanDef.Type && p.Name == beanDef.Name);

            foreach (var registration in matchedList)
            {
                registration.LifetimeManager.RemoveValue();

                container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) =>
                {
                    throw new NoSuchBeanDefinitionException($"Type {beanDef.Type} with name '{beanDef.Name}' is unregistered.");
                }, (IFactoryLifetimeManager)Activator.CreateInstance(registration.LifetimeManager.GetType()));
            }
        }
    }

    public static class ServicePostRegistryExt
    {
        public static void AddSingleton<TMap, TImpl>(this IServicePostRegistry servicePostRegistry, string name = null)
        {
            servicePostRegistry.AddSingleton(typeof(TMap), typeof(TImpl), name);
        }

        public static void AddSingleton<TMap>(this IServicePostRegistry servicePostRegistry, TMap instance, string name = null)
        {
            servicePostRegistry.AddSingleton(typeof(TMap), instance, name);
        }

        public static void AddSingleton<TMap>(this IServicePostRegistry servicePostRegistry, Func<IServiceProvider, Type, string, TMap> factory, string name = null)
        {
            servicePostRegistry.AddSingleton(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }

        public static void AddTransient<TMap, TImpl>(this IServicePostRegistry servicePostRegistry, string name = null)
        {
            servicePostRegistry.AddTransient(typeof(TMap), typeof(TImpl), name);
        }

        public static void AddTransient<TMap>(this IServicePostRegistry servicePostRegistry, Func<IServiceProvider, Type, string, TMap> factory, string name = null)
        {
            servicePostRegistry.AddTransient(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }

        public static void Unregister<TMap>(this IServicePostRegistry servicePostRegistry, string name = null)
        {
            servicePostRegistry.Unregister(typeof(TMap), name);
        }
    }
}
