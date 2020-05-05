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

namespace UnityAddon.Core.Context
{
    public class CoreContext
    {
        private readonly IUnityContainer _container = new UnityContainer();

        private readonly ApplicationContext _applicationContext;

        public CoreContext(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
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
                .RegisterInstance(_applicationContext, new ContainerControlledLifetimeManager());

            var sp = _container
                .RegisterType<ServiceProvider>(new ContainerControlledLifetimeManager(), new InjectionConstructor(_applicationContext.AppContainer))
                .Resolve<ServiceProvider>();
            _container.RegisterInstance<IServiceProvider>(sp, new ContainerControlledLifetimeManager())
                .RegisterInstance<IServiceScopeFactory>(sp, new ContainerControlledLifetimeManager())
                .RegisterInstance<IServiceScope>(sp, new ContainerControlledLifetimeManager());

            _container
                .RegisterType<ProxyGenerator>(new ContainerControlledLifetimeManager())
                .RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<ParameterFill>(new ContainerControlledLifetimeManager())
                .RegisterType<PropertyFill>(new ContainerControlledLifetimeManager())
                .RegisterType<DependencyResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<BeanMethodInterceptor>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<IServicePostRegistry, ServicePostRegistry>(new ContainerControlledLifetimeManager());

            _container
                .RegisterType<BeanBuildStrategyExtension>(new ContainerControlledLifetimeManager());

            return _container;
        }
    }
}
