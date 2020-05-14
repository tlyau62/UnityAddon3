using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.Config;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Context
{
    public class ApplicationContext
    {
        private readonly CoreContext _coreContext;

        public ApplicationContext(IUnityContainer appContainer)
        {
            _coreContext = new CoreContext(this, appContainer);
        }

        public IUnityContainer CoreContainer => _coreContext.Container;

        public IUnityAddonSP ApplicationSP => CoreContainer.Resolve<IUnityAddonSP>();

        public IBeanDefinitionContainer BeanDefinitionContainer => CoreContainer.Resolve<IBeanDefinitionContainer>();

        public ConfigurationRegistry ConfigurationRegistry => CoreContainer.Resolve<ConfigurationRegistry>();

        public BeanDefinitionRegistry BeanDefinitionRegistry => CoreContainer.Resolve<BeanDefinitionRegistry>();

        public void ConfigureBeans(Action<IBeanDefinitionCollection> config)
        {
            var defCol = new BeanDefinitionCollection();

            config(defCol);

            ConfigureBeans(defCol);
        }

        public void ConfigureBeans(IBeanDefinitionCollection defCollection)
        {
            foreach (var beanDef in defCollection)
            {
                BeanDefinitionContainer.RegisterBeanDefinition(beanDef);
                ApplicationSP.UnityContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(new UnityAddonSP(c), t, n), (IFactoryLifetimeManager)beanDef.Scope);
            }
        }

        //public void ConfigureBeans(Action<IBeanDefinitionCollection, IUnityAddonSP> config, ApplicationContextEntryOrder order)
        //{
        //    AddContextEntry(entry =>
        //    {
        //        entry.Order = order;
        //        entry.Process = sp =>
        //        {
        //            var defCol = new BeanDefinitionCollection();
        //            var candidateSelector = ApplicationSP.GetService<BeanDefintionCandidateSelector>();

        //            config(defCol, ApplicationSP);

        //            foreach (var beanDef in defCol)
        //            {
        //                if (!(candidateSelector?.Filter(beanDef) ?? true))
        //                {
        //                    continue;
        //                }

        //                BeanDefinitionContainer.RegisterBeanDefinition(beanDef);
        //                ApplicationSP.UnityContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(new UnityAddonSP(c), t, n), (IFactoryLifetimeManager)beanDef.Scope);

        //                if (beanDef.Type == typeof(IBeanDefinitionCollection))
        //                {
        //                    ConfigureBeans((config, sp) =>
        //                    {
        //                        config.AddFromExisting(ApplicationSP.GetRequiredService<IBeanDefinitionCollection>(beanDef.Name));
        //                    }, ApplicationContextEntryOrder.BeanMethod);
        //                }
        //            }

        //            ConfigurationRegistry.RefreshConfigurations();
        //        };

        //    });
        //}

        //public void AddContextEntry(IAppContainerBuilderEntry entry)
        //{
        //    var wrapEntry = new ApplicationContextEntry(entry.Order);

        //    wrapEntry.PreProcess += (container) =>
        //    {
        //        entry.PreProcess(container);
        //        entry.ConfigureBeanDefinitions(wrapEntry.BeanDefinitionCollection);
        //    };
        //    wrapEntry.PostProcess += (container) => entry.PostProcess(container);

        //    _entries.Add(wrapEntry);
        //}

        public void ConfigureContext<TConfig>(Action<TConfig> config) where TConfig : class, new()
        {
            _coreContext.Configure(config);
        }

        //public void Refresh()
        //{
        //    if (_isRefreshing)
        //    {
        //        return;
        //    }

        //    _isRefreshing = true;

        //    while (_entries.Count > 0)
        //    {
        //        var min = _entries.DeleteMin();

        //        min.Process(ApplicationSP);
        //    }

        //    _isRefreshing = false;
        //}

        public IUnityAddonSP Build()
        {
            ApplicationSP.UnityContainer.AddExtension(CoreContainer.Resolve<BeanBuildStrategyExtension>());

            ConfigureBeans(config => config.AddFromUnityContainer(CoreContainer));

            //// beandefintion candidate selector
            //ConfigureBeans((config, sp) =>
            //    config.AddSingleton<BeanDefintionCandidateSelector, BeanDefintionCandidateSelector>(), ApplicationContextEntryOrder.AppPreConfig);

            ApplicationSP.UnityContainer.AddExtension(CoreContainer.Resolve<AopBuildStrategyExtension>());

            PostRegistry();

            CoreContainer.Resolve<AopInterceptorContainer>().Init();

            PreInstantiateSingleton();

            return CoreContainer.Resolve<IUnityAddonSP>();
        }

        public void PostRegistry(int? count = null)
        {
            if (count == null)
            {
                count = 0;
            }

            var newCount = ApplicationSP.UnityContainer.Registrations.Count();

            if (count == newCount)
            {
                return;
            }

            count = newCount;
            ConfigurationRegistry.Refresh();
            BeanDefinitionRegistry.Refresh();
            PostRegistry(count);
        }

        public void ParseBeanDefintionCollection(HashSet<IBeanDefinition> hashSet = null)
        {
            var hasNew = false;

            if (hashSet == null)
            {
                hashSet = new HashSet<IBeanDefinition>();
            }

            foreach (var colDef in BeanDefinitionContainer.GetAllBeanDefinitions(typeof(IBeanDefinitionCollection)))
            {
                if (hashSet.Contains(colDef))
                {
                    continue;
                }

                hashSet.Add(colDef);

                ConfigureBeans(config => config.AddFromExisting(ApplicationSP.GetRequiredService<IBeanDefinitionCollection>(colDef.Name)));

                hasNew = true;
            }

            if (hasNew)
            {
                ParseBeanDefintionCollection(hashSet);
            }
        }

        public void PreInstantiateSingleton()
        {
            var sp = ApplicationSP;
            var container = ApplicationSP.UnityContainer;
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations.ToArray())
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager) || reg.Name.EndsWith("factory")) // skip bean method factory bean
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {

                    sp.GetRequiredService(reg.RegisteredType, reg.Name);
                }
            }

            if (container.Registrations.Count() != currentRegs)
            {
                PreInstantiateSingleton();
            }
        }
    }
}
