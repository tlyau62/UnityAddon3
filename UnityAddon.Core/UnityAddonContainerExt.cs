using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core
{
    public static class UnityAddonContainerExt
    {
        public static bool IsRegisteredUA<T>(this IUnityContainer container, string name = null)
        {
            return container.IsRegisteredUA(typeof(T), name);
        }

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container, string name, ITypeLifetimeManager lifetimeManager)
        {
            return container.RegisterTypeUA(name, typeof(ResolveType), typeof(ImplType), lifetimeManager);
        }

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container)
        {
            return container.RegisterTypeUA<ResolveType, ImplType>(null, new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager)
        {
            return container.RegisterTypeUA<ResolveType, ImplType>(null, lifetimeManager);
        }

        public static T ResolveUA<T>(this IUnityContainer container, string name = null)
        {
            return (T)container.ResolveUA(typeof(T), name);
        }

        public static object ResolveUA(this IUnityContainer container, Type type)
        {
            return container.ResolveUA(type, null);
        }

        public static T ResolveOptionalUA<T>(this IUnityContainer container, string name = null)
        {
            return (T)container.ResolveOptionalUA(typeof(T), name);
        }

        public static IUnityContainer RegisterInstanceUA<T>(this IUnityContainer container, T instance)
        {
            return container.RegisterInstanceUA(instance, null);
        }

        public static IUnityContainer RegisterInstanceUA<T>(this IUnityContainer container, T instance, string name)
        {
            return container.RegisterInstanceUA(typeof(T), instance, name);
        }

        public static IUnityContainer RegisterFactoryUA<T>(this IUnityContainer container, Func<IUnityContainer, Type, string, T> factory, IFactoryLifetimeManager lifetimeManager)
            where T : class
        {
            return container.RegisterFactoryUA(typeof(T), null, factory, lifetimeManager);
        }

        public static IUnityContainer RegisterFactoryUA<T>(this IUnityContainer container, Func<IUnityContainer, Type, string, T> factory)
            where T : class
        {
            return container.RegisterFactoryUA(typeof(T), null, factory, new ContainerControlledLifetimeManager());
        }

        public static void UnregisterUA<T>(this IUnityContainer container, string name = null)
        {
            container.UnregisterUA(typeof(T), name);
        }

        public static IEnumerable<object> ResolveAllUA(this IUnityContainer container, Type type)
        {
            var resolveAll = typeof(UnityAddonContainer)
                .GetMethod("ResolveAllUA", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .MakeGenericMethod(type);

            try
            {
                return (IEnumerable<object>)resolveAll.Invoke(null, new object[] { container });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
