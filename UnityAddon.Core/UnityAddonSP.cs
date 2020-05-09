using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core
{
    public interface IUnityAddonSP : IServiceProvider, ISupportRequiredService, IServiceScopeFactory, IServiceScope
    {
        string Id { get; }
        IUnityContainer UnityContainer { get; }
        object GetService(Type serviceType, string name = null);
        T GetService<T>(string name = null);
        object GetRequiredService(Type serviceType, string name = null);
        T GetRequiredService<T>(string name = null);
        bool CanResolve(Type serviceType, string name = null);
        bool CanResolve<T>(string name = null);
        bool IsRegistered(Type serviceType, string name = null);
        bool IsRegistered<T>(string name = null);
        object BuildUp(Type type, object service);
        T BuildUp<T>(T service);
        IEnumerable<string> GetBeanNames(Type serviceType);
        IEnumerable<string> GetBeanNames<T>();
    }

    public class UnityAddonSP : IUnityAddonSP, IDisposable
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public UnityAddonSP(IUnityContainer container)
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

        public T GetService<T>(string name)
        {
            return (T)GetService(typeof(T), name);
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

        public T GetRequiredService<T>(string name)
        {
            return (T)GetRequiredService(typeof(T), name);
        }

        public void Dispose()
        {
            IDisposable disposable = UnityContainer;
            UnityContainer = null;
            disposable?.Dispose();
        }

        public IServiceScope CreateScope()
        {
            return new UnityAddonSP(UnityContainer.CreateChildContainer());
        }

        public bool CanResolve(Type serviceType, string name)
        {
            return IsRegistered(serviceType, name) || serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public bool CanResolve<T>(string name)
        {
            return CanResolve(typeof(T), name);
        }

        public bool IsRegistered(Type serviceType, string name)
        {
            var beanDefContainer = UnityContainer.Resolve<IBeanDefinitionContainer>();

            return beanDefContainer.HasBeanDefinition(serviceType, name) || serviceType.IsGenericType &&
                beanDefContainer.HasBeanDefinition(serviceType.GetGenericTypeDefinition(), name);
        }

        public bool IsRegistered<T>(string name = null)
        {
            return IsRegistered(typeof(T), name);
        }

        public object BuildUp(Type type, object service)
        {
            return UnityContainer.Resolve<PropertyFill>().FillAllProperties(type, service, this);
        }

        public T BuildUp<T>(T service)
        {
            return (T)BuildUp(typeof(T), service);
        }

        public IEnumerable<string> GetBeanNames(Type serviceType)
        {
            return UnityContainer.Resolve<IBeanDefinitionContainer>()
                .GetAllBeanDefinitions(serviceType)
                .Select(def => def.Name)
                .ToArray();
        }

        public IEnumerable<string> GetBeanNames<T>()
        {
            return GetBeanNames(typeof(T));
        }

        public override string ToString()
        {
            return Id;
        }

        IServiceProvider IServiceScope.ServiceProvider => this;
    }
}
