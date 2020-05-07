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
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new SingletonLifetimeManager());

            _container
                .RegisterInstance(_applicationContext, new SingletonLifetimeManager());

            var sp = _container
                .RegisterType<ServiceProvider>(new SingletonLifetimeManager(), new InjectionConstructor(_applicationContext.AppContainer))
                .Resolve<ServiceProvider>();
            _container.RegisterInstance<IServiceProvider>(sp, new SingletonLifetimeManager())
                .RegisterInstance<IServiceScopeFactory>(sp, new SingletonLifetimeManager())
                .RegisterInstance<IServiceScope>(sp, new SingletonLifetimeManager());

            _container
                .RegisterType<ProxyGenerator>(new SingletonLifetimeManager())
                .RegisterType<ConstructorResolver>(new SingletonLifetimeManager())
                .RegisterType<ParameterFill>(new SingletonLifetimeManager())
                .RegisterType<PropertyFill>(new SingletonLifetimeManager())
                .RegisterType<DependencyResolver>(new SingletonLifetimeManager())
                .RegisterType<BeanFactory>(new SingletonLifetimeManager());

            _container
                .RegisterType<BeanMethodInterceptor>(new SingletonLifetimeManager());

            _container
                .RegisterType<IServicePostRegistry, ServicePostRegistry>(new SingletonLifetimeManager());

            _container
                .RegisterType<BeanBuildStrategyExtension>(new SingletonLifetimeManager());

            _container
                .RegisterType(typeof(IConfigs<>), typeof(Configs<>), new SingletonLifetimeManager());
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
