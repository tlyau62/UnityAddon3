using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Unity;
using Unity.Lifetime;
using UnityAddon.Bean;
using UnityAddon.BeanBuildStrategies;

namespace UnityAddon
{
    public class ServiceProviderFactory : IServiceProviderFactory<IUnityContainer>
    {
        private readonly IUnityContainer _container;

        private readonly IBeanDefinitionContainer _beanDefContainer;

        private readonly ConstructorResolver _ctorResolver;

        private readonly IServiceProvider _sp;

        public ServiceProviderFactory() : this(new UnityContainer())
        {
        }

        public ServiceProviderFactory(IUnityContainer container)
        {
            _container = container;

            _container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IServiceProvider, ServiceProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager());

            _beanDefContainer = container.Resolve<IBeanDefinitionContainer>();
            _sp = _container.Resolve<IServiceProvider>();
            _ctorResolver = container.Resolve<ConstructorResolver>();
        }

        public IUnityContainer CreateBuilder(IServiceCollection services)
        {
            // sp
            services.AddSingleton(_sp);
            services.AddSingleton<IServiceScopeFactory>((ServiceProvider)_sp);
            services.AddSingleton<IServiceScope>((ServiceProvider)_sp);

            // internal
            services.AddSingleton(_beanDefContainer);
            services.AddSingleton(_ctorResolver);

            _container.AddNewExtension<BeanBuildStrategyExtension>();

            foreach (var d in services)
            {
                IBeanDefinition beanDef = null;

                if (d.ImplementationInstance != null)
                {
                    beanDef = new ServiceInstanceBeanDefinition(d);
                }
                else if (d.ImplementationFactory != null)
                {
                    beanDef = new ServiceFactoryBeanDefinition(d);
                }
                else if (d.ImplementationType != null)
                {
                    beanDef = new ServiceTypeBeanDefinition(d);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                _beanDefContainer.RegisterBeanDefinition(beanDef);
                _container.RegisterFactory(beanDef.BeanType, beanDef.BeanName, beanDef.Constructor, (IFactoryLifetimeManager)beanDef.BeanScope);
            }

            return _container;
        }

        public IServiceProvider CreateServiceProvider(IUnityContainer container)
        {
            return container.Resolve<IServiceProvider>();
        }
    }
}


