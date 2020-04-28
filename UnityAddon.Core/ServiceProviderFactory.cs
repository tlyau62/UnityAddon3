using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.ServiceBeanDefinition;
using UnityAddon.Core.Component;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon
{
    public class ServiceProviderFactory : IServiceProviderFactory<IUnityContainer>
    {
        private readonly IUnityContainer _container;

        private readonly IBeanDefinitionContainer _beanDefContainer;

        private readonly ConstructorResolver _ctorResolver;

        private readonly IServiceProvider _sp;

        private readonly BeanFactory _beanFactory;

        private readonly IThreadLocalFactory<Stack<ResolveStackEntry>> _threadLocalResolveStack;

        private bool _isNewContainer = false;

        public ServiceProviderFactory() : this(new UnityContainer())
        {
            _isNewContainer = true;
        }

        public ServiceProviderFactory(IUnityContainer container)
        {
            _container = container;

            _container
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .RegisterType<IServiceProvider, ServiceProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<ConstructorResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<IThreadLocalFactory<Stack<ResolveStackEntry>>, ThreadLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));

            _beanDefContainer = container.Resolve<IBeanDefinitionContainer>();
            _sp = _container.Resolve<IServiceProvider>();
            _ctorResolver = container.Resolve<ConstructorResolver>();
            _beanFactory = container.Resolve<BeanFactory>();
            _threadLocalResolveStack = container.Resolve<IThreadLocalFactory<Stack<ResolveStackEntry>>>();
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
            services.AddSingleton(_beanFactory);
            services.AddSingleton<ValueProvider>();
            services.AddSingleton<ConfigBracketParser>();
            services.AddSingleton(_threadLocalResolveStack);
            services.AddSingleton(sp => sp.GetService<AopInterceptorContainerBuilder>()?.Build(sp));
            services.AddSingleton(sp => sp.GetService<BeanDefintionCandidateSelectorBuilder>().Build(sp.GetService<IConfiguration>()));
            services.AddSingleton(sp => sp.GetService<ComponentScannerBuilder>().Build(sp));

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
                _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
            }

            var beanDefFilters = _sp.GetRequiredService<BeanDefintionCandidateSelector>();
            var compScanner = _sp.GetRequiredService<ComponentScanner>();
            var compScannedDefs = _sp.GetRequiredService<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>().SelectMany(cb => cb(compScanner));
            var beanDefCollection = compScannedDefs.Where(d => !d.FromComponentScanning || beanDefFilters.Filter(d));
            var configParser = new ConfigurationParser();

            beanDefCollection = beanDefCollection.Union(configParser.Parse(beanDefCollection));

            _beanDefContainer.RegisterBeanDefinitions(beanDefCollection);

            _container.AddNewExtension<AopBuildStrategyExtension>();

            foreach (var beanDef in beanDefCollection)
            {
                _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
            }

            foreach (var defCollection in _sp.GetServices<IBeanDefinitionCollection>())
            {
                _beanDefContainer.RegisterBeanDefinitions(defCollection);

                foreach (var beanDef in defCollection)
                {
                    _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
            }

            if (_isNewContainer)
            {
                _sp.GetService<IHostApplicationLifetime>()?.ApplicationStopped.Register(() => _container.Dispose());
            }

            return _container;
        }

        public IServiceProvider CreateServiceProvider(IUnityContainer container)
        {
            return container.Resolve<IServiceProvider>();
        }
    }
}


