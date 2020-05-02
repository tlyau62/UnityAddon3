using Microsoft.Extensions.DependencyInjection;
using System;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public class BeanLoaderMainEntry : IBeanLoaderEntry
    {
        private IBeanDefinitionContainer _definitionContainer;

        private IServiceProvider _sp;

        public BeanLoaderEntryOrder Order => BeanLoaderEntryOrder.Intern;

        public bool PreInstantiate => true;

        public void ConfigureBeanDefinitions(IBeanDefinitionCollection collection)
        {
            collection.AddFromServiceCollection(services =>
            {
                services
                    .AddSingleton(_sp)
                    .AddSingleton((IServiceScopeFactory)_sp)
                    .AddSingleton((IServiceScope)_sp)
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

            _sp = container.RegisterType<UnityAddonServiceProvider>(new ContainerControlledLifetimeManager())
                    .RegisterFactory<IServiceProvider>(c => c.Resolve<UnityAddonServiceProvider>())
                    .RegisterFactory<IServiceScopeFactory>(c => c.Resolve<UnityAddonServiceProvider>())
                    .RegisterFactory<IServiceScope>(c => c.Resolve<UnityAddonServiceProvider>())
                    .Resolve<IServiceProvider>();
        }
    }
}
