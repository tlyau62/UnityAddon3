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

namespace UnityAddon.Core.Bootstrap
{

    public class ContainerBuilder
    {
        private readonly IntervalHeap<ContainerBuilderEntry> _entries = new IntervalHeap<ContainerBuilderEntry>(Comparer<ContainerBuilderEntry>.Create((a, b) => a.Order - b.Order));

        private readonly IUnityContainer _appContainer;

        private readonly BootstrapContainerBuilder _coreContainerBuilder;

        private IUnityContainer _coreContainer;

        public ContainerBuilder(IUnityContainer appContainer)
        {
            _appContainer = appContainer;
            _coreContainerBuilder = new BootstrapContainerBuilder(appContainer);
        }

        public void AddContextEntry(Action<ContainerBuilderEntry> entryConfig)
        {
            AddContextEntry(ContainerBuilderEntryOrder.App, false, entryConfig);
        }

        public void AddContextEntry(ContainerBuilderEntryOrder order, bool preInstantiate, Action<ContainerBuilderEntry> entryConfig)
        {
            var entry = new ContainerBuilderEntry(order, preInstantiate);

            entryConfig(entry);

            _entries.Add(entry);
        }

        public void AddContextEntry(ContainerBuilderEntry entry)
        {
            _entries.Add(entry);
        }

        public void AddContextEntry(IContainerBuilderEntry entry)
        {
            var wrapEntry = new ContainerBuilderEntry(entry.Order, entry.PreInstantiate);

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

            AddContextEntry(ContainerBuilderEntryOrder.Intern, false, entry =>
            {
                entry.ConfigureBeanDefinitions(defs => defs.AddFromUnityContainer(_coreContainer));
            });

            Refresh();

            var a = _coreContainer.Resolve<IBeanDefinitionContainer>() == _coreContainer.Resolve<IBeanDefinitionContainer>();

            var z = _appContainer.Resolve<IBeanDefinitionContainer>();

            return _coreContainer.Resolve<IServiceProvider>();
        }

        private void Register(ContainerBuilderEntry loadEntry, ContainerBuilderEntryOrder curOrder)
        {
            var child = _appContainer.CreateChildContainer();

            loadEntry.PreProcess(_appContainer);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(ContainerBuilderEntry))
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
                    if (beanDef.Type != typeof(ContainerBuilderEntry))
                    {
                        _appContainer.Resolve(beanDef.Type, beanDef.Name);
                    }
                }
            }

            loadEntry.PostProcess(_appContainer);

            foreach (var entry in child.ResolveAll<ContainerBuilderEntry>())
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
