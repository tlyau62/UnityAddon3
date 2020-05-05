using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bootstrap
{
    public class BootstrapContainerBuilder
    {
        private readonly IUnityContainer _container = new UnityContainer();

        private readonly IUnityContainer _appContainer;

        public BootstrapContainerBuilder(IUnityContainer appContainer)
        {
            _appContainer = appContainer;
        }

        public void Configure<TConfig>(Action<TConfig> config)
        {
            if (!_container.IsRegistered<TConfig>())
            {
                _container.RegisterType<TConfig>(new ContainerControlledLifetimeManager());
            }

            config(_container.Resolve<TConfig>());
        }

        public IUnityContainer Build()
        {
            _container
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<ServiceProvider>(new ContainerControlledLifetimeManager(), new InjectionConstructor(_appContainer))
                .RegisterFactory<IServiceProvider>(c => _container.Resolve<ServiceProvider>(), new ContainerControlledLifetimeManager())
                .RegisterFactory<IServiceScopeFactory>(c => _container.Resolve<ServiceProvider>(), new ContainerControlledLifetimeManager())
                .RegisterFactory<IServiceScope>(c => _container.Resolve<ServiceProvider>(), new ContainerControlledLifetimeManager());

            _container
                .RegisterType<ProxyGenerator>(new ContainerControlledLifetimeManager())
                .RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<ParameterFill>(new ContainerControlledLifetimeManager())
                .RegisterType<PropertyFill>(new ContainerControlledLifetimeManager())
                .RegisterType<DependencyResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<ValueProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<ConfigBracketParser>(new ContainerControlledLifetimeManager())
                .RegisterType<BeanMethodInterceptor>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<ServicePostRegistry>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<BeanBuildStrategyExtension>(new ContainerControlledLifetimeManager());

            return _container;
        }
    }
}
