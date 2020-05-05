using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Thread;

namespace UnityAddon.Core.Context
{
    public class ApplicationContext
    {
        private readonly IntervalHeap<ApplicationContextEntry> _entries = new IntervalHeap<ApplicationContextEntry>(Comparer<ApplicationContextEntry>.Create((a, b) => a.Order - b.Order));

        private readonly IUnityContainer _appContainer;

        private readonly CoreContext _coreContainerBuilder;

        private IUnityContainer _coreContainer;

        public ApplicationContext(IUnityContainer appContainer)
        {
            _appContainer = appContainer;
            _coreContainerBuilder = new CoreContext(this);
        }

        public IUnityContainer AppContainer => _appContainer;

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
            while (_entries.Count > 0)
            {
                var min = _entries.DeleteMin();

                Register(min, min.Order);
            }
        }

        public IServiceProvider Build()
        {
            _coreContainer = _coreContainerBuilder.Build();

            _appContainer.AddExtension(_coreContainer.Resolve<BeanBuildStrategyExtension>());

            AddContextEntry(ApplicationContextEntryOrder.Intern, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs => defs.AddFromUnityContainer(_coreContainer));
            });

            Refresh();

            return _coreContainer.Resolve<IServiceProvider>();
        }

        private void Register(ApplicationContextEntry loadEntry, ApplicationContextEntryOrder curOrder)
        {
            var child = _appContainer.CreateChildContainer();

            loadEntry.PreProcess(_appContainer);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(ApplicationContextEntry))
                {
                    child.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_coreContainer.Resolve<IServiceProvider>(), t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
                else
                {
                    _coreContainer.Resolve<IBeanDefinitionContainer>().RegisterBeanDefinition(beanDef);
                    _appContainer.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_coreContainer.Resolve<IServiceProvider>(), t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
            }

            if (loadEntry.PreInstantiate)
            {
                foreach (var beanDef in loadEntry.BeanDefinitionCollection)
                {
                    if (beanDef.Type != typeof(ApplicationContextEntry))
                    {
                        _appContainer.Resolve(beanDef.Type, beanDef.Name);
                    }
                }
            }

            loadEntry.PostProcess(_appContainer);

            foreach (var entry in child.ResolveAll<ApplicationContextEntry>())
            {
                if (entry.Order <= curOrder)
                {
                    Register(entry, curOrder);
                }
                else
                {
                    _entries.Add(entry);
                }
            }

            child.Dispose();
        }
    }
}
