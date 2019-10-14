using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Exceptions;

namespace UnityAddon
{
    public interface IContainerRegistry
    {
        void RegisterType<TRegType, TMapType>(string name);
        void RegisterType<TRegType, TMapType>(ITypeLifetimeManager lifetimeManager);
        void RegisterType<TRegType, TMapType>(string name, ITypeLifetimeManager lifetimeManager);
        void RegisterType(Type regType, Type mapType, string name, ITypeLifetimeManager lifetimeManager);

        void RegisterInstance<TInstanceType>(TInstanceType instance);

        T Resolve<T>(string name = null);
        object Resolve(Type type, string name = null);

        T[] ResolveAll<T>();
        object[] ResolveAll(Type type);

        bool IsRegistered<T>(string name = null);
        bool IsRegistered(Type type, string name);

        T BuildUp<T>(T existing, string name = null);
        object BuildUp(Type type, object existing, string name = null);
    }

    [Component]
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

        public bool IsRegistered(Type type, string name)
        {
            type = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            var isRegistered = Container.IsRegistered(type) || Container.IsRegistered(type, name);

            foreach (var reg in Container.Registrations)
            {
                if (reg.RegisteredType == type && (name == null || reg.Name == name))
                {
                    return true;
                }
            }

            return BeanDefinitionContainer.HasBeanDefinition(type, name);
        }

        public T Resolve<T>(string name = null)
        {
            return (T)Container.Resolve(typeof(T), name);
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
                Container.RegisterInstance(instance);
            }
        }

        public T BuildUp<T>(T existing, string name = null)
        {
            lock (Container)
            {
                return Container.BuildUp(existing, name);
            }
        }

        public object BuildUp(Type type, object existing, string name = null)
        {
            var buildUpMethod = GetType().GetMethods().Where(m => m.Name == nameof(BuildUp) && m.IsGenericMethod).Single();

            return buildUpMethod
                .MakeGenericMethod(new[] { type })
                .Invoke(this, new[] { (dynamic)existing, name });
        }
    }
}
