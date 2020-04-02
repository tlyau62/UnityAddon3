using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace UnityAddon.Core
{
    public static class UnityAddonContainerExt
    {
        public static bool IsRegisteredUA<T>(this IUnityContainer container, string name = null)
        {
            return container.IsRegisteredUA(typeof(T), name);
        }

        public static void RegisterTypeUA<T>(this IUnityContainer container, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            container.RegisterTypeUA(typeof(T), name, lifetimeManager, injectionMembers);
        }

        public static void RegisterTypeUA<T>(this IUnityContainer container, params InjectionMember[] injectionMembers)
        {
            container.RegisterTypeUA(typeof(T), null, new ContainerControlledTransientManager(), injectionMembers);
        }

        public static void RegisterTypeUA<T>(this IUnityContainer container, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            container.RegisterTypeUA(typeof(T), null, lifetimeManager, injectionMembers);
        }

        public static T ResolveUA<T>(this IUnityContainer container, string name = null)
        {
            return (T)container.ResolveUA(typeof(T), name);
        }
    }
}
