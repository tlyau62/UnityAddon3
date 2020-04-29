using Castle.DynamicProxy;
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

namespace UnityAddon.Core
{
    public class ServiceProviderFactory : IServiceProviderFactory<IUnityContainer>
    {
        private readonly IUnityContainer _container;

        private readonly IBeanDefinitionContainer _beanDefContainer;

        private readonly ConstructorResolver _ctorResolver;

        private readonly IServiceProvider _sp;

        private readonly BeanFactory _beanFactory;

        private readonly IThreadLocalFactory<Stack<ResolveStackEntry>> _threadLocalResolveStack;

        private readonly IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>> _scannedBeanDefinitions;

        private bool _isNewContainer = false;

        private IServiceCollection _servicesCollection;

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
                .RegisterType<IThreadLocalFactory<Stack<ResolveStackEntry>>, ThreadLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())))
                .RegisterType<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>, List<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>(new ContainerControlledLifetimeManager());

            _beanDefContainer = container.Resolve<IBeanDefinitionContainer>();
            _sp = _container.Resolve<IServiceProvider>();
            _ctorResolver = container.Resolve<ConstructorResolver>();
            _beanFactory = container.Resolve<BeanFactory>();
            _threadLocalResolveStack = container.Resolve<IThreadLocalFactory<Stack<ResolveStackEntry>>>();
            _scannedBeanDefinitions = container.Resolve<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>();
        }

        public IUnityContainer CreateBuilder(IServiceCollection services)
        {
            _servicesCollection = services;

            return _container;
        }

        public IServiceProvider CreateServiceProvider(IUnityContainer container)
        {
            // sp
            _servicesCollection.AddSingleton(_sp);
            _servicesCollection.AddSingleton<IServiceScopeFactory>((ServiceProvider)_sp);
            _servicesCollection.AddSingleton<IServiceScope>((ServiceProvider)_sp);

            // internal
            _servicesCollection.AddSingleton(_beanDefContainer);
            _servicesCollection.AddSingleton(_ctorResolver);
            _servicesCollection.AddSingleton(_beanFactory);
            _servicesCollection.AddSingleton<ValueProvider>();
            _servicesCollection.AddSingleton<ConfigBracketParser>();
            _servicesCollection.AddSingleton(_threadLocalResolveStack);
            _servicesCollection.AddSingleton(sp => (sp.GetService<AopInterceptorContainerBuilder>() ?? new AopInterceptorContainerBuilder()).Build(sp));
            _servicesCollection.AddSingleton(sp => (sp.GetService<BeanDefintionCandidateSelectorBuilder>() ?? new BeanDefintionCandidateSelectorBuilder()).Build(sp.GetService<IConfiguration>()));
            _servicesCollection.AddSingleton(sp => (sp.GetService<ComponentScannerBuilder>() ?? new ComponentScannerBuilder()).Build(sp));
            _servicesCollection.AddSingleton(_scannedBeanDefinitions);
            _servicesCollection.AddSingleton<AopBuildStrategyExtension>();
            _servicesCollection.AddSingleton<BeanAopStrategy>();
            _servicesCollection.AddSingleton<AopMethodBootstrapInterceptor>();
            _servicesCollection.AddSingleton<InterfaceProxyFactory>();
            _servicesCollection.AddSingleton<ProxyGenerator>();

            _container.AddNewExtension<BeanBuildStrategyExtension>();

            foreach (var d in _servicesCollection)
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


            return _sp;
        }
    }
}


