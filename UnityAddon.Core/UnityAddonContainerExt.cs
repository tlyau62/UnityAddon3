using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return container.RegisterTypeUA(name, typeof(ResolveType), typeof(ImplType), lifetimeManager, injectionMembers);
        }

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            return container.RegisterTypeUA<ResolveType, ImplType>(null, new ContainerControlledLifetimeManager(), injectionMembers);
        }

        public static IUnityContainer RegisterTypeUA<ResolveType, ImplType>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return container.RegisterTypeUA<ResolveType, ImplType>(null, lifetimeManager, injectionMembers);
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

        public static IUnityContainer RegisterFactoryUA<T>(this IUnityContainer container, Func<IUnityContainer, Type, string, T> factory, IFactoryLifetimeManager lifetimeManager)
        {
            return container.RegisterFactoryUA(null, factory, lifetimeManager);
        }

        public static IUnityContainer RegisterFactoryUA<T>(this IUnityContainer container, Func<IUnityContainer, Type, string, T> factory)
        {
            return container.RegisterFactoryUA(null, factory, new ContainerControlledLifetimeManager());
        }

        public static void UnregisterUA<T>(this IUnityContainer container, string name = null)
        {
            container.UnregisterUA(typeof(T), name);
        }

        public static IEnumerable<T> ResolveAllUA<T>(this IUnityContainer container)
        {
            return container.ResolveAllUA(typeof(T)).Cast<T>();
        }
    }
}
