using Microsoft.Extensions.DependencyInjection;
using System;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public class MainEntry : IContainerBuilderEntry
    {
        private IBeanDefinitionContainer _definitionContainer;

        private IServiceProvider _sp;

        private ContainerBuilder _containerBuilder;

        public MainEntry(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public ContainerBuilderEntryOrder Order => ContainerBuilderEntryOrder.Intern;

        public bool PreInstantiate => true;

        public void ConfigureBeanDefinitions(IBeanDefinitionCollection collection)
        {
            collection.AddFromServiceCollection(services =>
            {
                services
                    .AddSingleton(_sp)
                    .AddSingleton((IServiceScopeFactory)_sp)
                    .AddSingleton((IServiceScope)_sp)
                    .AddSingleton(_containerBuilder)
                    .AddSingleton(_definitionContainer);
            });
        }

        public void PostProcess(IUnityContainer container)
        {
            container.AddNewExtension<BeanBuildStrategyExtension>();
        }

        public void PreProcess(IUnityContainer container)
        {
            _definitionContainer = container
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .Resolve<IBeanDefinitionContainer>();

            _sp = container.RegisterType<ServiceProvider>(new ContainerControlledLifetimeManager())
                    .RegisterFactory<IServiceProvider>(c => c.Resolve<ServiceProvider>())
                    .RegisterFactory<IServiceScopeFactory>(c => c.Resolve<ServiceProvider>())
                    .RegisterFactory<IServiceScope>(c => c.Resolve<ServiceProvider>())
                    .Resolve<IServiceProvider>();
        }
    }
}
