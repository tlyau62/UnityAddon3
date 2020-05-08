﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core
{
    public interface IUnityServiceProvider
    {
        IUnityContainer UnityContainer { get; }

        bool CanResolve(Type serviceType, string name);

        bool IsRegistered(Type serviceType, string name);

        object BuildUp(Type type, object service);

        IEnumerable<string> GetBeanNames(Type serviceType);
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

        public static T BuildUp<T>(this IServiceProvider sp, T service)
        {
            return (T)((IUnityServiceProvider)sp).BuildUp(typeof(T), service);
        }

        public static object BuildUp(this IServiceProvider sp, Type type, object service)
        {
            return ((IUnityServiceProvider)sp).BuildUp(type, service);
        }

        public static IEnumerable<string> GetBeanNames<T>(this IServiceProvider sp)
        {
            return ((IUnityServiceProvider)sp).GetBeanNames(typeof(T));
        }
    }

    // Support name interface
    public class ServiceProvider : IServiceProvider, IDisposable, ISupportRequiredService, IServiceScopeFactory, IServiceScope, IUnityServiceProvider, ISupportNamedService
    {
        private string _uuid = Guid.NewGuid().ToString();

        public ServiceProvider(IUnityContainer container)
        {
            UnityContainer = container;
        }

        public IUnityContainer UnityContainer { get; private set; }

        public object GetService(Type serviceType)
        {
            return GetService(serviceType, null);
        }

        public object GetService(Type serviceType, string name)
        {
            return UnityContainer.Resolve(serviceType, name);
        }

        public object GetRequiredService(Type serviceType)
        {
            return GetRequiredService(serviceType, null);
        }

        public object GetRequiredService(Type serviceType, string name)
        {
            if (!CanResolve(serviceType, name))
            {
                throw new NoSuchBeanDefinitionException($"Type {serviceType} with name \"{name}\" cannot be found.");
            }

            return GetService(serviceType, name);
        }

        public void Dispose()
        {
            // UnityContainer.Dispose();
        }

        public IServiceScope CreateScope()
        {
            return new ServiceProvider(UnityContainer.CreateChildContainer());
        }

        public bool CanResolve(Type serviceType, string name)
        {
            return IsRegistered(serviceType, name) || serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public bool IsRegistered(Type serviceType, string name)
        {
            var beanDefContainer = UnityContainer.Resolve<IBeanDefinitionContainer>();

            return beanDefContainer.HasBeanDefinition(serviceType, name) || serviceType.IsGenericType &&
                beanDefContainer.HasBeanDefinition(serviceType.GetGenericTypeDefinition(), name);
        }

        public object BuildUp(Type type, object service)
        {
            return UnityContainer.Resolve<PropertyFill>().FillAllProperties(type, service, this);
        }

        public IEnumerable<string> GetBeanNames(Type serviceType)
        {
            return UnityContainer.Resolve<IBeanDefinitionContainer>()
                .GetAllBeanDefinitions(serviceType)
                .Select(def => def.Name)
                .ToArray();
        }

        public override string ToString()
        {
            return _uuid;
        }

        IServiceProvider IServiceScope.ServiceProvider => this;
    }
}
