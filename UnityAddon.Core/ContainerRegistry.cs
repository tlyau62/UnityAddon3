using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core
{
    /// <summary>
    /// A replace of unity container, but with thread-safe and easy mocking.
    /// </summary>
    public interface IContainerRegistry
    {
        void RegisterType<TRegType, TMapType>(string name);
        void RegisterType<TRegType, TMapType>(ITypeLifetimeManager lifetimeManager);
        void RegisterType<TRegType, TMapType>(string name, ITypeLifetimeManager lifetimeManager);
        void RegisterType(Type regType, Type mapType, string name, ITypeLifetimeManager lifetimeManager);

        void RegisterInstance<TInstanceType>(TInstanceType instance);
        void RegisterInstance<TInstanceType>(TInstanceType instance, string name);

        T Resolve<T>(string name = null);
        object Resolve(Type type, string name = null);

        T[] ResolveAll<T>();
        object[] ResolveAll(Type type);

        bool IsRegistered<T>(string name = null);
        bool IsRegistered(Type type, string name = null);

        T BuildUp<T>(T existing, string name = null);
        object BuildUp(Type type, object existing, string name = null); 
    }

    /// <summary>
    /// Thread-safe
    /// </summary>
    public class ContainerRegistry : IContainerRegistry
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public void RegisterType<TRegType, TMapType>(string name)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), name, null);
        }

        public void RegisterType<TRegType, TMapType>(ITypeLifetimeManager lifetimeManager)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), null, lifetimeManager);
        }

        public void RegisterType<TRegType, TMapType>(string name, ITypeLifetimeManager lifetimeManager)
        {
            RegisterType(typeof(TRegType), typeof(TMapType), name, lifetimeManager);
        }

        public void RegisterType(Type regType, Type mapType, string name, ITypeLifetimeManager lifetimeManager)
        {
            lock (Container)
            {
                Container.RegisterType(regType, mapType, name, lifetimeManager);
            }
        }

        public bool IsRegistered<T>(string name = null)
        {
            return IsRegistered(typeof(T), name);
        }

        public bool IsRegistered(Type type, string name = null)
        {
            foreach (var reg in Container.Registrations)
            {
                if ((reg.RegisteredType == type) && (name == null || reg.Name == name))
                {
                    return true;
                }
                else if (type.IsGenericType && reg.RegisteredType == type.GetGenericTypeDefinition() && (name == null || reg.Name == name)) // fallback on generic type definition
                {
                    return true;
                }
            }

            return BeanDefinitionContainer.HasBeanDefinition(type, name) || (type.IsGenericType && BeanDefinitionContainer.HasBeanDefinition(type.GetGenericTypeDefinition(), name));
        }

        public T Resolve<T>(string name = null)
        {
            return (T)Resolve(typeof(T), name);
        }

        public object Resolve(Type type, string name = null)
        {
            if (!IsRegistered(type, name))
            {
                throw new NoSuchBeanDefinitionException($"Type {type} with name '{name}' is not registered.");
            }

            return Container.Resolve(type, name);
        }

        public T[] ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Cast<T>().ToArray();
        }

        /// <summary>
        /// By unity default, bean without name is not included in ResolveAll.
        /// </summary>
        public object[] ResolveAll(Type type)
        {
            List<object> beans = new List<object>();

            if (BeanDefinitionContainer.HasBeanDefinition(type))
            {
                foreach (var beanDef in BeanDefinitionContainer.GetAllBeanDefinitions(type))
                {
                    if (beanDef.GetBeanQualifiers().Length > 0)
                    {
                        beans.Add(Container.Resolve(beanDef.GetBeanType(), beanDef.GetBeanName()));
                    }
                }
            }

            foreach (var reg in Container.Registrations)
            {
                if (reg.RegisteredType == type && reg.Name != null)
                {
                    if (!BeanDefinitionContainer.HasBeanDefinition(type, reg.Name))
                    {
                        beans.Add(Container.Resolve(type, reg.Name));
                    }
                }
            }

            return beans.ToArray();
        }

        public void RegisterInstance<TInstanceType>(TInstanceType instance)
        {
            lock (Container)
            {
                Container.RegisterInstance(instance, new ContainerControlledLifetimeManager());
            }
        }

        public void RegisterInstance<TInstanceType>(TInstanceType instance, string name)
        {
            lock (Container)
            {
                Container.RegisterInstance(typeof(TInstanceType), name, instance, new ContainerControlledLifetimeManager());
            }
        }

        /// <summary>
        /// Use PropertyFill for internal use
        /// </summary>
        public T BuildUp<T>(T existing, string name = null)
        {
            lock (Container)
            {
                return Container.BuildUp(existing, name);
            }
        }

        /// <summary>
        /// Use PropertyFill for internal use
        /// </summary>
        public object BuildUp(Type type, object existing, string name = null)
        {
            var buildUpMethod = GetType().GetMethods().Where(m => m.Name == nameof(BuildUp) && m.IsGenericMethod).Single();

            return buildUpMethod
                .MakeGenericMethod(new[] { type })
                .Invoke(this, new[] { (dynamic)existing, name });
        }
    }
}
