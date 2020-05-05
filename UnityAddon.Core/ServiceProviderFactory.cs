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
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Bootstrap;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core
{
    public class ServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly ContainerBuilder _beanLoader;

        private bool _isNewContainer = false;

        public ServiceProviderFactory() : this(new UnityContainer())
        {
            _isNewContainer = true;
        }

        public ServiceProviderFactory(IUnityContainer container)
        {
            _beanLoader = new ContainerBuilder(container);
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services = null)
        {
            var beanAppEntry = new ContainerBuilderEntry(ContainerBuilderEntryOrder.NetAsp, false);

            beanAppEntry.ConfigureBeanDefinitions(defs =>
            {
                defs.AddFromServiceCollection(services);
            });

            _beanLoader.AddContextEntry(beanAppEntry);

            return _beanLoader;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder beanDefCol)
        {
            return _beanLoader.Build();

            //var beanDefFilters = _sp.GetRequiredService<BeanDefintionCandidateSelector>();
            //var beanDefCollection = compScannedDefs.Where(d => !d.FromComponentScanning || beanDefFilters.Filter(d));

            //_beanDefContainer.RegisterBeanDefinitions(beanDefCollection);

            //_container.AddNewExtension<AopBuildStrategyExtension>();

            //foreach (var beanDef in beanDefCollection)
            //{
            //    _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
            //}

            //foreach (var defCollection in _sp.GetServices<IBeanDefinitionCollection>())
            //{
            //    _beanDefContainer.RegisterBeanDefinitions(defCollection);

            //    foreach (var beanDef in defCollection)
            //    {
            //        _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
            //    }
            //}

            //var _sp = _container.Resolve<IServiceProvider>();

            //if (_isNewContainer)
            //{
            //    _sp.GetService<IHostApplicationLifetime>()?.ApplicationStopped.Register(() => _container.Dispose());
            //}

            //return _sp;
        }
    }
}


