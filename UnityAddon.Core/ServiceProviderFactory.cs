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
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core
{
    public class ServiceProviderFactory : IServiceProviderFactory<ApplicationContext>
    {
        private readonly IUnityContainer _container;

        private bool _isNewContainer = false;

        public ServiceProviderFactory() : this(new UnityContainer())
        {
            _isNewContainer = true;
        }

        public ServiceProviderFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ApplicationContext CreateBuilder(IServiceCollection services = null)
        {
            var appCtx = new ApplicationContext(_container);

            appCtx.AddContextEntry(ApplicationContextEntryOrder.NetAsp, false,
                entry => entry.ConfigureBeanDefinitions(defs => defs.AddFromServiceCollection(services)));

            return appCtx;
        }

        public IServiceProvider CreateServiceProvider(ApplicationContext appCtx)
        {
            return appCtx.Build();

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


