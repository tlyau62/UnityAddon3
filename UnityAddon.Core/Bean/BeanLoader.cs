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

    public class BeanLoader
    {
        private readonly IntervalHeap<BeanLoaderEntry> _entries = new IntervalHeap<BeanLoaderEntry>(Comparer<BeanLoaderEntry>.Create((a, b) => a.Order - b.Order));

        private readonly IUnityContainer _container;

        public BeanLoader() : this(new UnityContainer()) { }

        public BeanLoader(IUnityContainer container)
        {
            _container = container;

            Add(new BeanLoaderMainEntry());
            Add(new BeanLoaderConstructEntry());
            Add(new BeanLoaderResolveEntry());
        }

        public void Add(BeanLoaderEntry entry)
        {
            _entries.Add(entry);
        }

        public void Add(IBeanLoaderEntry entry)
        {
            var wrapEntry = new BeanLoaderEntry(entry.Order, entry.PreInstantiate);

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

        private void Register(BeanLoaderEntry loadEntry, BeanLoaderEntryOrder curOrder)
        {
            var child = _container.CreateChildContainer();

            loadEntry.PreProcess(_container);

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(BeanLoader))
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
                    if (beanDef.Type != typeof(BeanLoaderEntry))
                    {
                        _container.Resolve(beanDef.Type, beanDef.Name);
                    }
                }
            }

            loadEntry.PostProcess(_container);

            foreach (var entry in child.ResolveAll<BeanLoaderEntry>())
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
