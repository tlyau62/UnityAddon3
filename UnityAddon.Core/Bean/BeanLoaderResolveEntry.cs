using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bean
{
    public class BeanLoaderResolveEntry : IBeanLoaderEntry
    {
        public BeanLoaderEntryOrder Order => BeanLoaderEntryOrder.Intern;

        public bool PreInstantiate => false;

        public void ConfigureBeanDefinitions(IBeanDefinitionCollection collection)
        {
            collection.AddFromServiceCollection(services =>
            {
                services.AddSingleton<ValueProvider>()
                    .AddSingleton<ConfigBracketParser>()
                    .AddSingleton<AopBuildStrategyExtension>()
                    .AddSingleton<BeanAopStrategy>()
                    .AddSingleton<AopMethodBootstrapInterceptor>()
                    .AddSingleton<InterfaceProxyFactory>()
                    .AddSingleton<ProxyGenerator>()
                    .AddSingleton<BeanMethodInterceptor>()
                    .AddSingleton(sp => (sp.GetService<AopInterceptorContainerBuilder>() ?? new AopInterceptorContainerBuilder()).Build(sp))
                    .AddSingleton(sp => (sp.GetService<BeanDefintionCandidateSelectorBuilder>() ?? new BeanDefintionCandidateSelectorBuilder()).Build(sp.GetService<IConfiguration>()));
            });
        }

        public void PostProcess(IUnityContainer container)
        {
        }

        public void PreProcess(IUnityContainer container)
        {
        }
    }
}
