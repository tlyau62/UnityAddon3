using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Thread;

namespace UnityAddon.Core.Bean
{

    public class ContainerBuilder
    {
        private readonly IntervalHeap<ContainerBuilderEntry> _entries = new IntervalHeap<ContainerBuilderEntry>(Comparer<ContainerBuilderEntry>.Create((a, b) => a.Order - b.Order));

        private readonly IUnityContainer _container;

        private readonly IUnityContainer _configContainer = new UnityContainer();

        public ContainerBuilder(IUnityContainer container)
        {
            _container = container;

            AddContextEntry(new MainEntry(this));
            AddContextEntry(new ConstructEntry());
            AddContextEntry(new ResolveEntry());

            // post register
            AddContextEntry(new ContainerBuilderEntry(ContainerBuilderEntryOrder.Intern, false).ConfigureBeanDefinitions(config =>
            {
                config.AddComponent(typeof(ServicePostRegistry));
            }));
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

            wrapEntry.PreProcess += (container, configContainer) =>
            {
                entry.PreProcess(container, configContainer);
                entry.ConfigureBeanDefinitions(wrapEntry.BeanDefinitionCollection);
            };
            wrapEntry.PostProcess += (container, configContainer) => entry.PostProcess(container, configContainer);

            _entries.Add(wrapEntry);
        }

        public void ConfigureContext<TConfig>(Action<TConfig> config)
        {
            _configContainer.RegisterType<TConfig>(new ContainerControlledLifetimeManager());

            config(_configContainer.Resolve<TConfig>());
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
            Refresh();

            return new ServiceProvider(_container);
        }

        private void Register(ContainerBuilderEntry loadEntry, ContainerBuilderEntryOrder curOrder)
        {
            var child = _container.CreateChildContainer();

            loadEntry.PreProcess(_container, _configContainer);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(ContainerBuilderEntry))
                {
                    child.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(c.Resolve<IServiceProvider>(), t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
                else
                {
                    _container.Resolve<IBeanDefinitionContainer>().RegisterBeanDefinition(beanDef);
                    _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(c.Resolve<IServiceProvider>(), t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
            }

            if (loadEntry.PreInstantiate)
            {
                foreach (var beanDef in loadEntry.BeanDefinitionCollection)
                {
                    if (beanDef.Type != typeof(ContainerBuilderEntry))
                    {
                        _container.Resolve(beanDef.Type, beanDef.Name);
                    }
                }
            }

            loadEntry.PostProcess(_container, _configContainer);

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
