﻿using C5;
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
        private readonly IntervalHeap<ApplicationContextEntry> _entries = new IntervalHeap<ApplicationContextEntry>(Comparer<ApplicationContextEntry>.Create((a, b) => a.Order - b.Order));

        private readonly CoreContext _coreContext;

        private bool _isRefreshing = false;

        private System.Collections.Generic.HashSet<IBeanDefinitionCollection> HashSet { get; set; } = new System.Collections.Generic.HashSet<IBeanDefinitionCollection>();

        public ApplicationContext(IUnityContainer appContainer)
        {
            _coreContext = new CoreContext(this, appContainer);
        }

        public IUnityContainer CoreContainer => _coreContext.Container;

        public IUnityAddonSP ApplicationSP => CoreContainer.Resolve<IUnityAddonSP>();

        public IBeanDefinitionContainer BeanDefinitionContainer => CoreContainer.Resolve<IBeanDefinitionContainer>();

        public ConfigurationRegistry ConfigurationRegistry => CoreContainer.Resolve<ConfigurationRegistry>();

        public void AddContextEntry(Action<ApplicationContextEntry> entryConfig)
        {
            var entry = new ApplicationContextEntry();

            entryConfig(entry);

            _entries.Add(entry);
        }

        public void AddContextEntry(ApplicationContextEntry entry)
        {
            _entries.Add(entry);
        }

        public void ConfigureBeans(Action<IBeanDefinitionCollection, IUnityAddonSP> config)
        {
            ConfigureBeans(config, ApplicationContextEntryOrder.App);
        }

        public void ConfigureBeans(Action<IBeanDefinitionCollection, IUnityAddonSP> config, ApplicationContextEntryOrder order)
        {
            AddContextEntry(entry =>
            {
                entry.Order = order;
                entry.Process = sp =>
                {
                    var defCol = new BeanDefinitionCollection();
                    var candidateSelector = ApplicationSP.GetService<BeanDefintionCandidateSelector>();

                    config(defCol, ApplicationSP);

                    foreach (var beanDef in defCol)
                    {
                        if (!(candidateSelector?.Filter(beanDef) ?? true))
                        {
                            continue;
                        }

                        BeanDefinitionContainer.RegisterBeanDefinition(beanDef);
                        ApplicationSP.UnityContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(new UnityAddonSP(c), t, n), (IFactoryLifetimeManager)beanDef.Scope);

                        if (beanDef.Type == typeof(IBeanDefinitionCollection))
                        {
                            ConfigureBeans((config, sp) =>
                            {
                                config.AddFromExisting(ApplicationSP.GetRequiredService<IBeanDefinitionCollection>(beanDef.Name));
                            }, ApplicationContextEntryOrder.BeanMethod);
                        }
                    }

                    ConfigurationRegistry.RefreshConfigurations();
                };

            });
        }

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

        public void Refresh()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            while (_entries.Count > 0)
            {
                var min = _entries.DeleteMin();

                min.Process(ApplicationSP);
            }

            _isRefreshing = false;
        }

        public IUnityAddonSP Build()
        {
            ApplicationSP.UnityContainer.AddExtension(CoreContainer.Resolve<BeanBuildStrategyExtension>());

            ConfigureBeans((config, sp) => config.AddFromUnityContainer(CoreContainer), ApplicationContextEntryOrder.Intern);

            // add value resolve logic
            ConfigureBeans((config, sp) =>
            {
                if (!sp.IsRegistered<IConfiguration>())
                {
                    return;
                }
                config.AddSingleton<ValueProvider, ValueProvider>();
                config.AddSingleton<ConfigBracketParser, ConfigBracketParser>();

                ConfigureContext<DependencyResolverOption>(config
                    => config.AddResolveStrategy<ValueAttribute>((type, attr, sp)
                        => sp.GetRequiredService<ValueProvider>().GetValue(type, attr.Value)));
            }, ApplicationContextEntryOrder.AppPreConfig);

            // beandefintion candidate selector
            ConfigureBeans((config, sp) =>
                config.AddSingleton<BeanDefintionCandidateSelector, BeanDefintionCandidateSelector>(), ApplicationContextEntryOrder.AppPreConfig);

            // aop
            ConfigureBeans((config, sp) =>
            {
                config.AddSingleton<AopInterceptorContainer, AopInterceptorContainer>();
                config.AddSingleton<AopBuildStrategyExtension, AopBuildStrategyExtension>();
                config.AddSingleton<AopMethodBootstrapInterceptor, AopMethodBootstrapInterceptor>();
                config.AddSingleton<InterfaceProxyFactory, InterfaceProxyFactory>();
                config.AddSingleton<BeanAopStrategy, BeanAopStrategy>();
            }, ApplicationContextEntryOrder.AppPreConfig + 1);

            ConfigureBeans((config, sp) =>
            {
                sp.UnityContainer.AddExtension(sp.GetRequiredService<AopBuildStrategyExtension>());
            }, ApplicationContextEntryOrder.AppPreConfig + 2);

            AddContextEntry(entry =>
            {
                entry.Order = ApplicationContextEntryOrder.AppPostConfig;
                entry.Process += sp =>
                {
                    sp.GetRequiredService<AopInterceptorContainer>().Init();
                    return;
                };
            });

            Refresh();

            PreInstantiateSingleton();

            return CoreContainer.Resolve<IUnityAddonSP>();
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
