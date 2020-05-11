using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.BeanDefinition.ServiceBean;

namespace UnityAddon.Core.Bean
{
    public interface IBeanRegistry
    {
        void Add(IBeanDefinition beanDefinition);

        void AddSingleton(Type type, Type implType, string name = null);

        void AddSingleton<TMap, TImpl>(string name = null) where TImpl : class;

        void AddSingleton(Type type, object instance, string name = null);

        void AddSingleton<TMap>(TMap instance, string name = null);

        void AddSingleton(Type type, Func<IUnityAddonSP, Type, string, object> factory, string name = null);

        void AddSingleton<TMap>(Func<IUnityAddonSP, Type, string, TMap> factory, string name = null);

        void AddTransient(Type type, Type implType, string name = null);

        void AddTransient<TMap, TImpl>(string name = null) where TImpl : class;

        void AddTransient(Type type, Func<IUnityAddonSP, Type, string, object> factory, string name = null);

        void AddTransient<TMap>(Func<IUnityAddonSP, Type, string, TMap> factory, string name = null);

        void AddComponent<TImpl>() where TImpl : class;

        void AddComponent(Type type);

        void AddFromExisting(IEnumerable<IBeanDefinition> beanDefCollection);

        void AddFromServiceCollection(Action<IServiceCollection> servicesCallback);

        void AddFromServiceCollection(IServiceCollection services);

        void AddFromUnityContainer(IUnityContainer unityContainer);
    }

    public abstract class BeanRegistry : IBeanRegistry
    {
        public abstract void Add(IBeanDefinition beanDefinition);

        public void AddSingleton(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap, TImpl>(string name) where TImpl : class
        {
            AddSingleton(typeof(TMap), typeof(TImpl), name);
        }

        public void AddSingleton(Type type, object instance, string name)
        {
            Add(new InstanceBeanDefintion(type, instance, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap>(TMap instance, string name)
        {
            AddSingleton(typeof(TMap), instance, name);
        }

        public void AddSingleton(Type type, Func<IUnityAddonSP, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Singleton));
        }

        public void AddSingleton<TMap>(Func<IUnityAddonSP, Type, string, TMap> factory, string name)
        {
            AddSingleton(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }

        public void AddTransient(Type type, Type implType, string name)
        {
            Add(new TypeBeanDefintion(type, implType, name, ScopeType.Transient));
        }

        public void AddTransient<TMap, TImpl>(string name) where TImpl : class
        {
            AddTransient(typeof(TMap), typeof(TImpl), name);
        }

        public void AddTransient(Type type, Func<IUnityAddonSP, Type, string, object> factory, string name)
        {
            Add(new FactoryBeanDefinition(type, factory, name, ScopeType.Transient));
        }

        public void AddTransient<TMap>(Func<IUnityAddonSP, Type, string, TMap> factory, string name)
        {
            AddTransient(typeof(TMap), (sp, t, n) => factory(sp, t, n), name);
        }

        public void AddComponent(Type type)
        {
            Add(new MemberComponentBeanDefinition(type));
        }

        public void AddFromServiceCollection(Action<IServiceCollection> servicesCallback)
        {
            var services = new ServiceCollection();

            servicesCallback(services);

            AddFromServiceCollection(services);
        }

        public void AddFromServiceCollection(IServiceCollection services)
        {
            foreach (var d in services)
            {
                ServiceBeanDefinition beanDef = null;

                if (d.ImplementationInstance != null)
                {
                    beanDef = new ServiceInstanceBeanDefinition(d);
                }
                else if (d.ImplementationFactory != null)
                {
                    beanDef = new ServiceFactoryBeanDefinition(d);
                }
                else if (d.ImplementationType != null)
                {
                    beanDef = new ServiceTypeBeanDefinition(d);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                Add(beanDef);
            }
        }

        public void AddFromExisting(IEnumerable<IBeanDefinition> beanDefCollection)
        {
            foreach (var beanDef in beanDefCollection)
            {
                Add(beanDef);
            }
        }

        public void AddFromUnityContainer(IUnityContainer unityContainer)
        {
            foreach (var reg in unityContainer.Registrations)
            {
                if (reg.RegisteredType == typeof(IUnityContainer))
                {
                    continue;
                }

                var scope = ScopeType.None;

                if (reg.LifetimeManager is ContainerControlledLifetimeManager || reg.LifetimeManager is SingletonLifetimeManager)
                {
                    scope = ScopeType.Singleton;
                }
                else if (reg.LifetimeManager is ContainerControlledTransientManager)
                {
                    scope = ScopeType.Transient;
                }
                else if (reg.LifetimeManager is HierarchicalLifetimeManager)
                {
                    scope = ScopeType.Scoped;
                }
                else
                {
                    throw new NotImplementedException();
                }

                Add(new FactoryBeanDefinition(reg.RegisteredType, (sp, t, n) => unityContainer.Resolve(t, reg.Name), reg.Name, scope));
            }
        }

        public void AddComponent<TImpl>() where TImpl : class
        {
            AddComponent(typeof(TImpl));
        }
    }
}
