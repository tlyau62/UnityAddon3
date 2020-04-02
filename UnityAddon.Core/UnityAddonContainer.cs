using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core
{
    public static class UnityAddonContainer
    {
        public static void RegisterTypeUA(this IUnityContainer container, Type implType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var beanDefContainer = container.ResolveUA<IBeanDefinitionContainer>();

            beanDefContainer.RegisterBeanDefinition(new SimpleBeanDefinition(implType, name));

            container.RegisterType(implType, lifetimeManager, injectionMembers);
        }

        public static object ResolveUA(this IUnityContainer container, Type type, string name)
        {
            var defContainer = container.Resolve<IBeanDefinitionContainer>();

            if (!defContainer.HasBeanDefinition(type, name))
            {
                throw new NoSuchBeanDefinitionException($"Type {type} with name '{name}' is not registered.");
            }

            return container.Resolve(type, name);
        }
    }
}
