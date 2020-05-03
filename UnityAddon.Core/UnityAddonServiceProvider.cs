using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Unity;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core
{
    public interface IUnityServiceProvider
    {
        bool CanResolve(Type serviceType, string name);

        bool IsRegistered(Type serviceType, string name);

        object BuildUp(object service);
    }

    public interface ISupportNamedService
    {
        object GetService(Type serviceType, string name);

        object GetRequiredService(Type serviceType, string name);
    }

    public static class UnityServiceProvider
    {
        public static object GetRequiredService(this IServiceProvider sp, Type serviceType, string name)
        {
            return ((ISupportNamedService)sp).GetRequiredService(serviceType, name);
        }

        public static T GetRequiredService<T>(this IServiceProvider sp, string name)
        {
            return (T)sp.GetRequiredService(typeof(T), name);
        }

        public static object GetService(this IServiceProvider sp, Type serviceType, string name)
        {
            return ((ISupportNamedService)sp).GetService(serviceType, name);
        }

        public static T GetService<T>(this IServiceProvider sp, string name)
        {
            return (T)sp.GetService(typeof(T), name);
        }

        public static bool CanResolve(this IServiceProvider sp, Type serviceType, string name = null)
        {
            return ((IUnityServiceProvider)sp).CanResolve(serviceType, name);
        }

        public static bool CanResolve<T>(this IServiceProvider sp, string name = null)
        {
            return sp.CanResolve(typeof(T), name);
        }

        public static bool IsRegistered(this IServiceProvider sp, Type serviceType, string name = null)
        {
            return ((IUnityServiceProvider)sp).IsRegistered(serviceType, name);
        }

        public static bool IsRegistered<T>(this IServiceProvider sp, string name = null)
        {
            return sp.IsRegistered(typeof(T), name);
        }

        public static object BuildUp(this IServiceProvider sp, object service)
        {
            return ((IUnityServiceProvider)sp).BuildUp(service);
        }
    }

    // Support name interface
    public class UnityAddonServiceProvider : IServiceProvider, IDisposable, ISupportRequiredService, IServiceScopeFactory, IServiceScope, IUnityServiceProvider, ISupportNamedService
    {
        private readonly IUnityContainer _container;

        public UnityAddonServiceProvider(IUnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return GetService(serviceType, null);
        }

        public object GetService(Type serviceType, string name)
        {
            return _container.Resolve(serviceType, name);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetRequiredService(serviceType, null);
        }

        public object GetRequiredService(Type serviceType, string name)
        {
            if (!CanResolve(serviceType, name))
            {
                throw new InvalidOperationException("Service not register");
            }

            return GetService(serviceType, name);
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public IServiceScope CreateScope()
        {
            return new UnityAddonServiceProvider(_container.CreateChildContainer());
        }

        public bool CanResolve(Type serviceType, string name)
        {
            return IsRegistered(serviceType, name) || serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public bool IsRegistered(Type serviceType, string name)
        {
            var beanDefContainer = _container.Resolve<IBeanDefinitionContainer>();

            return beanDefContainer.HasBeanDefinition(serviceType, name) || serviceType.IsGenericType &&
                beanDefContainer.HasBeanDefinition(serviceType.GetGenericTypeDefinition(), name);
        }

        public object BuildUp(object service)
        {
            return _container.Resolve<PropertyFill>().FillAllProperties(service, this);
        }

        IServiceProvider IServiceScope.ServiceProvider => this;
    }
}
