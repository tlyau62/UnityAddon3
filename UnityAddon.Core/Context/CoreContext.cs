using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.Config;
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

            Setup();
        }

        public void Setup()
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

            _container
                .RegisterType(typeof(IConfigs<>), typeof(Configs<>), new ContainerControlledLifetimeManager());
        }

        public void Configure<TConfig>(Action<TConfig> config) where TConfig : class, new()
        {
            config(_container.Resolve<IConfigs<TConfig>>().Value);
        }

        public IUnityContainer Build()
        {
            return _container;
        }
    }
}
