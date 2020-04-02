using System;
using System.Collections.Generic;
using System.Linq;
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
        public static bool IsRegisteredUA(this IUnityContainer container, Type type, string name = null)
        {
            var defContainer = container.Resolve<IBeanDefinitionContainer>();

            return defContainer.HasBeanDefinition(type, name) ||
                type.IsGenericType && defContainer.HasBeanDefinition(type.GetGenericTypeDefinition(), name);
        }

        public static void RegisterTypeUA(this IUnityContainer container, Type implType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var beanDefContainer = container.ResolveUA<IBeanDefinitionContainer>();

            beanDefContainer.RegisterBeanDefinition(new SimpleBeanDefinition(implType, name));

            container.RegisterType(implType, lifetimeManager, injectionMembers);
        }

        public static object ResolveUA(this IUnityContainer container, Type type, string name)
        {
            if (!container.IsRegisteredUA(type, name))
            {
                throw new NoSuchBeanDefinitionException($"Type {type} with name '{name}' is not registered.");
            }

            object bean = container.Resolve(type, name);

            if (!type.IsAssignableFrom(bean.GetType()))
            {
                throw new ResolutionFailedException(type, name, $"Cannot convert type from {bean.GetType()} to {type}");
            }

            return bean;
        }

        /// <summary>
        /// Instantiate singleton bean recursively.
        /// Some bean may do bean registration at postconstruct,
        /// so recursive needed.
        /// 
        /// The final number of un-registrations will be converge to 0,
        /// since each bean is postconstructed once only.
        /// </summary>
        public static void PreInstantiateSingleton(this IUnityContainer container)
        {
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations)
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager))
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {
                    container.Resolve(reg.RegisteredType, reg.Name);
                }
            }

            if (container.Registrations.Count() != currentRegs)
            {
                PreInstantiateSingleton(container);
            }
        }
    }
}
