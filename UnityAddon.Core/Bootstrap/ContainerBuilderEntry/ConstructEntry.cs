using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public class ConstructEntry : IContainerBuilderEntry
    {
        public ContainerBuilderEntryOrder Order => ContainerBuilderEntryOrder.Intern;

        public bool PreInstantiate => true;

        private DependencyResolver _dependencyResolver;

        private ProxyGenerator _proxyGenerator;

        private ConstructorResolver _constructorResolver;

        private ParameterFill _parameterFill;

        private PropertyFill _propertyFill;

        private BeanFactory _beanFactory;

        public void ConfigureBeanDefinitions(IBeanDefinitionCollection collection)
        {
            collection.AddFromServiceCollection(services =>
            {
                services
                    .AddSingleton(_dependencyResolver)
                    .AddSingleton(_proxyGenerator)
                    .AddSingleton(_constructorResolver)
                    .AddSingleton(_parameterFill)
                    .AddSingleton(_propertyFill)
                    .AddSingleton(_beanFactory);
            });
        }

        void IContainerBuilderEntry.PostProcess(IUnityContainer container)
        {
            container.AddNewExtension<BeanBuildStrategyExtension>();
        }

        void IContainerBuilderEntry.PreProcess(IUnityContainer container)
        {
            container.RegisterType<ProxyGenerator>(new ContainerControlledLifetimeManager())
                .RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<ParameterFill>(new ContainerControlledLifetimeManager())
                .RegisterType<PropertyFill>(new ContainerControlledLifetimeManager())
                .RegisterInstance(configContainer.Resolve<DependencyResolver>(), new ContainerControlledLifetimeManager())
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager());

            _dependencyResolver = container.Resolve<DependencyResolver>();
            _proxyGenerator = container.Resolve<ProxyGenerator>();
            _constructorResolver = container.Resolve<ConstructorResolver>();
            _parameterFill = container.Resolve<ParameterFill>();
            _propertyFill = container.Resolve<PropertyFill>();
            _beanFactory = container.Resolve<BeanFactory>();
        }
    }
}
