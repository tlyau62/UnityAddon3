using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Unity;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon
{
    public interface IUnityServiceProvider
    {
        bool CanResolve(Type serviceType, string name);

        bool IsRegistered(Type serviceType, string name);
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
            return (T)((ISupportNamedService)sp).GetRequiredService(typeof(T), name);
        }

        public static object GetService(this IServiceProvider sp, Type serviceType, string name)
        {
            return ((ISupportNamedService)sp).GetService(serviceType, name);
        }

        public static T GetService<T>(this IServiceProvider sp, string name)
        {
            return (T)((ISupportNamedService)sp).GetService(typeof(T), name);
        }

        public static bool CanResolve(this IServiceProvider sp, Type serviceType, string name = null)
        {
            return ((IUnityServiceProvider)sp).CanResolve(serviceType, name);
        }

        public static bool IsRegistered(this IServiceProvider sp, Type serviceType, string name = null)
        {
            return ((IUnityServiceProvider)sp).IsRegistered(serviceType, name);
        }
    }

    // Support name interface
    public class ServiceProvider : IServiceProvider, IDisposable, ISupportRequiredService, IServiceScopeFactory, IServiceScope, IUnityServiceProvider, ISupportNamedService
    {
        private readonly IUnityContainer _container;

        private readonly IBeanDefinitionContainer _beanDefContainer;

        public ServiceProvider(IUnityContainer container)
        {
            _container = container;
            _beanDefContainer = container.Resolve<IBeanDefinitionContainer>();
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
            return new ServiceProvider(_container.CreateChildContainer());
        }

        public bool CanResolve(Type serviceType, string name)
        {
            return IsRegistered(serviceType, name) || serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public bool IsRegistered(Type serviceType, string name)
        {
            return _beanDefContainer.HasBeanDefinition(serviceType, name) || serviceType.IsGenericType &&
                _beanDefContainer.HasBeanDefinition(serviceType.GetGenericTypeDefinition(), name);
        }

        IServiceProvider IServiceScope.ServiceProvider => this;
    }
}
