using Castle.DynamicProxy;
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

        public static IUnityContainer RegisterTypeUA(this IUnityContainer container, string name, Type resolveType, Type implType, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var def = new SimpleBeanDefinition(resolveType, name);

            container.ResolveUA<IBeanDefinitionContainer>()
                .RegisterBeanDefinition(def);

            return container.RegisterType(resolveType, implType, name, lifetimeManager, injectionMembers);
        }

        public static object ResolveUA(this IUnityContainer container, Type type, string name)
        {
            if (!container.IsRegisteredUA(type, name))
            {
                throw new NoSuchBeanDefinitionException($"Type {type} with name '{name}' is not registered.");
            }

            return container.ResolveOptionalUA(type, name);
        }

        public static object ResolveOptionalUA(this IUnityContainer container, Type type, string name)
        {
            if (!container.IsRegisteredUA(type, name))
            {
                return null;
            }

            object bean = container.Resolve(type, name);

            if (!type.IsAssignableFrom(ProxyUtil.IsProxy(bean) ? ProxyUtil.GetUnproxiedType(bean) : bean.GetType()))
            {
                throw new ResolutionFailedException(type, name, $"Cannot convert type from {bean.GetType()} to {type}");
            }

            return bean;
        }

        public static IUnityContainer RegisterInstanceUA(this IUnityContainer container, Type type, object instance, string name)
        {
            var beanDef = new SimpleBeanDefinition(type, name);

            container.Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinition(beanDef);

            container.RegisterInstance(type, name, instance);

            return container;
        }

        public static IUnityContainer RegisterFactoryUA(this IUnityContainer container, Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            var beanDef = new SimpleBeanDefinition(type, name);

            container.Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinition(beanDef);

            container.RegisterFactory(type, name, (c, t, n) => factory(c, t, n), lifetimeManager);

            return container;
        }

        public static IEnumerable<object> ResolveAllUA(this IUnityContainer container, Type type)
        {
            return container.Resolve<IBeanDefinitionContainer>()
                .GetAllBeanDefinitions(type)
                .Select(def => container.ResolveUA(def.BeanType, def.BeanName));
        }

        public static void UnregisterUA(this IUnityContainer container, Type type, string name = null)
        {
            var beanDefContainer = container.ResolveUA<IBeanDefinitionContainer>();
            var bean = container.ResolveUA(type, name); // ensure the bean exists
            var beanDef = beanDefContainer.RemoveBeanDefinition(type, name);

            var matchedList = container.Registrations.Where(p => p.RegisteredType == beanDef.BeanType && p.Name == beanDef.BeanName);

            foreach (var registration in matchedList)
            {
                registration.LifetimeManager.RemoveValue();

                container.RegisterFactory(beanDef.BeanType, beanDef.BeanName, (c, t, n) =>
                {
                    throw new NoSuchBeanDefinitionException($"Type {beanDef.BeanType} with name '{beanDef.BeanName}' is unregistered.");
                }, (IFactoryLifetimeManager)Activator.CreateInstance(registration.LifetimeManager.GetType()));
            }
        }

    }
}
