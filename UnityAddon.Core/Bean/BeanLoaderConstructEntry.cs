using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    public class BeanLoaderConstructEntry : IBeanLoaderEntry
    {
        public BeanLoaderEntryOrder Order => BeanLoaderEntryOrder.Intern;

        public bool PreInstantiate => true;

        public void ConfigureBeanDefinitions(IBeanDefinitionCollection collection)
        {
            var child = new UnityContainer();

            child.RegisterType<ProxyGenerator>(new ContainerControlledLifetimeManager())
                .RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<ParameterFill>(new ContainerControlledLifetimeManager())
                .RegisterType<PropertyFill>(new ContainerControlledLifetimeManager())
                .RegisterType<DependencyResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager());

            collection.AddFromServiceCollection(services =>
            {
                services
                    .AddSingleton(child.Resolve<ProxyGenerator>())
                    .AddSingleton(child.Resolve<ConstructorResolver>())
                    .AddSingleton(child.Resolve<ParameterFill>())
                    .AddSingleton(child.Resolve<PropertyFill>())
                    .AddSingleton(child.Resolve<DependencyResolver>())
                    .AddSingleton(child.Resolve<BeanFactory>());
            });
        }

        void IBeanLoaderEntry.PostProcess(IUnityContainer container)
        {
        }

        void IBeanLoaderEntry.PreProcess(IUnityContainer container)
        {
        }
    }
}
