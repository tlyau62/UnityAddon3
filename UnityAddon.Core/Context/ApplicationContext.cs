using C5;
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

        private readonly CoreContext _coreContainerBuilder;

        private IUnityContainer _coreContainer;

        private bool _isRefreshing = false;

        public ApplicationContext(IUnityContainer appContainer)
        {
            _coreContainerBuilder = new CoreContext(this);
            AppContainer = appContainer;
        }

        public IUnityContainer AppContainer { get; }

        public void AddContextEntry(Action<ApplicationContextEntry> entryConfig)
        {
            AddContextEntry(ApplicationContextEntryOrder.App, false, entryConfig);
        }

        public void AddContextEntry(ApplicationContextEntryOrder order, bool preInstantiate, Action<ApplicationContextEntry> entryConfig)
        {
            var entry = new ApplicationContextEntry(order, preInstantiate);

            entryConfig(entry);

            _entries.Add(entry);
        }

        public void AddContextEntry(ApplicationContextEntry entry)
        {
            _entries.Add(entry);
        }

        public void AddContextEntry(IAppContainerBuilderEntry entry)
        {
            var wrapEntry = new ApplicationContextEntry(entry.Order, entry.PreInstantiate);

            wrapEntry.PreProcess += (container) =>
            {
                entry.PreProcess(container);
                entry.ConfigureBeanDefinitions(wrapEntry.BeanDefinitionCollection);
            };
            wrapEntry.PostProcess += (container) => entry.PostProcess(container);

            _entries.Add(wrapEntry);
        }

        public void ConfigureContext<TConfig>(Action<TConfig> config)
        {
            _coreContainerBuilder.Configure(config);
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

                Register(min, min.Order);
            }

            _isRefreshing = false;
        }

        public IServiceProvider Build()
        {
            _coreContainer = _coreContainerBuilder.Build();

            AppContainer.AddExtension(_coreContainer.Resolve<BeanBuildStrategyExtension>());

            AddContextEntry(ApplicationContextEntryOrder.Intern, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs => defs.AddFromUnityContainer(_coreContainer));
            });

            // add value resolve logic
            AddContextEntry(ApplicationContextEntryOrder.NetAsp + 1, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs =>
                {
                    defs.Add(new TypeBeanDefintion(typeof(ValueProvider), typeof(ValueProvider), null, ScopeType.Singleton));
                    defs.Add(new TypeBeanDefintion(typeof(ConfigBracketParser), typeof(ConfigBracketParser), null, ScopeType.Singleton));
                });

                entry.PostProcess += c => ConfigureContext<DependencyResolverOption>(config =>
                {
                    config.AddResolveStrategy<ValueAttribute>((type, attr, sp) =>
                    {
                        return sp.GetRequiredService<ValueProvider>().GetValue(type, attr.Value);
                    });
                });
            });

            // beandefintion candidate selector
            AddContextEntry(ApplicationContextEntryOrder.NetAsp + 1, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs =>
                {
                    defs.Add(new TypeBeanDefintion(typeof(BeanDefintionCandidateSelector), typeof(BeanDefintionCandidateSelector), null, ScopeType.Singleton));
                });
            });

            // aop
            AddContextEntry(ApplicationContextEntryOrder.AppPreConfig, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs =>
                {
                    defs.Add(new TypeBeanDefintion(typeof(AopInterceptorContainer), typeof(AopInterceptorContainer), null, ScopeType.Singleton));
                    defs.Add(new TypeBeanDefintion(typeof(AopBuildStrategyExtension), typeof(AopBuildStrategyExtension), null, ScopeType.Singleton));
                    defs.Add(new TypeBeanDefintion(typeof(AopMethodBootstrapInterceptor), typeof(AopMethodBootstrapInterceptor), null, ScopeType.Singleton));
                    defs.Add(new TypeBeanDefintion(typeof(InterfaceProxyFactory), typeof(InterfaceProxyFactory), null, ScopeType.Singleton));
                    defs.Add(new TypeBeanDefintion(typeof(BeanAopStrategy), typeof(BeanAopStrategy), null, ScopeType.Singleton));
                });

                entry.PostProcess += c => c.AddExtension(c.Resolve<AopBuildStrategyExtension>());
            });

            AddContextEntry(ApplicationContextEntryOrder.AppPostConfig, false, entry =>
            {
                entry.PreProcess += c => c.Resolve<AopInterceptorContainer>().Init();
            });

            Refresh();

            return _coreContainer.Resolve<IServiceProvider>();
        }

        private void Register(ApplicationContextEntry loadEntry, ApplicationContextEntryOrder curOrder)
        {
            var sp = _coreContainer.Resolve<IServiceProvider>();

            loadEntry.PreProcess(AppContainer);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (!(sp.GetService<BeanDefintionCandidateSelector>()?.Filter(beanDef) ?? true))
                {
                    continue;
                }

                _coreContainer.Resolve<IBeanDefinitionContainer>().RegisterBeanDefinition(beanDef);
                AppContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(sp, t, n), (IFactoryLifetimeManager)beanDef.Scope);
            }

            if (loadEntry.BeanDefinitionCollection.Any(def => def.Type == typeof(IBeanDefinitionCollection)))
            {
                AddContextEntry(ApplicationContextEntryOrder.BeanMethod, false, entry =>
                {
                    entry.ConfigureBeanDefinitions(defCol =>
                    {
                        foreach (var beanDef in loadEntry.BeanDefinitionCollection.Where(def => def.Type == typeof(IBeanDefinitionCollection)))
                        {
                            defCol.AddFromExisting(sp.GetRequiredService<IBeanDefinitionCollection>(beanDef.Name));
                        }
                    });
                });
            }

            loadEntry.PostProcess(AppContainer);

            if (loadEntry.PreInstantiate)
            {
                foreach (var beanDef in loadEntry.BeanDefinitionCollection)
                {
                    AppContainer.Resolve(beanDef.Type, beanDef.Name);
                }
            }
        }
    }
}
