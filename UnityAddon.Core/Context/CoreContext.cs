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
        private readonly ApplicationContext _applicationContext;

        private readonly IUnityContainer _applicationContainer;

        public CoreContext(ApplicationContext applicationContext, IUnityContainer applicationContainer)
        {
            _applicationContext = applicationContext;
            _applicationContainer = applicationContainer;

            Setup();
        }

        public IUnityContainer Container { get; } = new UnityContainer();

        public void Setup()
        {
            Container
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new SingletonLifetimeManager());

            Container
                .RegisterInstance(_applicationContext, new SingletonLifetimeManager());

            var sp = Container
                .RegisterType<UnityAddonSP>(new SingletonLifetimeManager(), new InjectionConstructor(_applicationContainer))
                .Resolve<UnityAddonSP>();
            Container
                .RegisterInstance<IUnityAddonSP>(sp, new SingletonLifetimeManager())
                .RegisterInstance<IServiceProvider>(sp, new SingletonLifetimeManager())
                .RegisterInstance<IServiceScopeFactory>(sp, new SingletonLifetimeManager())
                .RegisterInstance<IServiceScope>(sp, new SingletonLifetimeManager());

            Container
                .RegisterType<ProxyGenerator>(new SingletonLifetimeManager())
                .RegisterType<ConstructorResolver>(new SingletonLifetimeManager())
                .RegisterType<ParameterFill>(new SingletonLifetimeManager())
                .RegisterType<PropertyFill>(new SingletonLifetimeManager())
                .RegisterType<DependencyResolver>(new SingletonLifetimeManager())
                .RegisterType<BeanFactory>(new SingletonLifetimeManager());

            Container
                .RegisterType<BeanMethodInterceptor>(new SingletonLifetimeManager());

            Container
                .RegisterType<IServicePostRegistry, ServicePostRegistry>(new SingletonLifetimeManager());

            Container
                .RegisterType<BeanBuildStrategyExtension>(new SingletonLifetimeManager());

            Container
                .RegisterType(typeof(IConfigs<>), typeof(Configs<>), new SingletonLifetimeManager());

            Container
                .RegisterType<ConfigurationRegistry>(new SingletonLifetimeManager());

            Container
                .RegisterType<AopInterceptorContainer>(new SingletonLifetimeManager())
                .RegisterType<AopBuildStrategyExtension>(new SingletonLifetimeManager())
                .RegisterType<AopMethodBootstrapInterceptor>(new SingletonLifetimeManager())
                .RegisterType<InterfaceProxyFactory>(new SingletonLifetimeManager())
                .RegisterType<BeanAopStrategy>(new SingletonLifetimeManager())
                .RegisterType<AopBuildStrategyExtension>(new SingletonLifetimeManager());
        }

        public void Configure<TConfig>(Action<TConfig> config) where TConfig : class, new()
        {
            var confBean = Container.Resolve<IConfigs<TConfig>>();

            config(confBean.Value);

            confBean.OnChange(confBean.Value);
        }
    }
}
