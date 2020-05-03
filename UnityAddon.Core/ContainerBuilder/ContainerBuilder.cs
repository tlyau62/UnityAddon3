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

        public ContainerBuilder() : this(new UnityContainer()) { }

        public ContainerBuilder(IUnityContainer container)
        {
            _container = container;

            Add(new MainEntry());
            Add(new ConstructEntry());
            Add(new ResolveEntry());
        }

        public void Add(ContainerBuilderEntry entry)
        {
            _entries.Add(entry);
        }

        public void Add(IContainerBuilderEntry entry)
        {
            var wrapEntry = new ContainerBuilderEntry(entry.Order, entry.PreInstantiate);

            wrapEntry.PreProcess += c =>
            {
                entry.PreProcess(c);
                wrapEntry.ConfigureBeanDefinitions(c => entry.ConfigureBeanDefinitions(c));
            };
            wrapEntry.PostProcess += c => entry.PostProcess(c);

            _entries.Add(wrapEntry);
        }

        public IServiceProvider Build()
        {
            while (_entries.Count > 0)
            {
                var min = _entries.DeleteMin();

                Register(min, min.Order);
            }

            return new UnityAddonServiceProvider(_container);
        }

        private void Register(ContainerBuilderEntry loadEntry, ContainerBuilderEntryOrder curOrder)
        {
            var child = _container.CreateChildContainer();

            loadEntry.PreProcess(_container);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(ContainerBuilder))
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

            loadEntry.PostProcess(_container);

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
