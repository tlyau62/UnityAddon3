using C5;
using Castle.DynamicProxy;
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
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bean
{
    public enum BeanLoaderEntryOrder
    {
        Intern = 0,
        NetAsp = 100,
        App = 200
    }

    public class BeanLoaderEntry
    {
        public BeanLoaderEntry()
        {
        }

        public BeanLoaderEntry(BeanLoaderEntryOrder order, bool preInstantiate)
        {
            Order = order;
            PreInstantiate = preInstantiate;
        }

        public BeanLoaderEntryOrder Order { get; set; } = BeanLoaderEntryOrder.App;

        public bool PreInstantiate { get; set; } = false;

        public IBeanDefinitionCollection BeanDefinitionCollection { get; } = new BeanDefinitionCollection();

        public Action<IUnityContainer> PreProcess { get; set; } = container => { };

        public Action<IUnityContainer> PostProcess { get; set; } = container => { };

        public BeanLoaderEntry ConfigureBeanDefinitions(Action<IBeanDefinitionCollection> config)
        {
            config(BeanDefinitionCollection);

            return this;
        }

        public BeanLoaderEntry ConfigureBeanDefinitions(Action<IServiceProvider, IBeanDefinitionCollection> config)
        {
            //config(container. BeanDefinitionCollection);

            throw new NotImplementedException();

            // return this;
        }
    }

    public class BeanLoader
    {
        private readonly IntervalHeap<BeanLoaderEntry> _entries = new IntervalHeap<BeanLoaderEntry>(Comparer<BeanLoaderEntry>.Create((a, b) => a.Order - b.Order));

        private readonly IUnityContainer _container;

        private readonly IServiceProvider _serviceProvider;

        private readonly IBeanDefinitionContainer _beanDefinitionContainer;

        public BeanLoader() : this(new UnityContainer()) { }

        public BeanLoader(IUnityContainer container)
        {
            _container = container;

            _container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .RegisterType<UnityAddonServiceProvider>(new ContainerControlledLifetimeManager())
                .RegisterFactory<IServiceProvider>(c => c.Resolve<UnityAddonServiceProvider>())
                .RegisterFactory<IServiceScopeFactory>(c => c.Resolve<UnityAddonServiceProvider>())
                .RegisterFactory<IServiceScope>(c => c.Resolve<UnityAddonServiceProvider>())
                .RegisterFactory<IThreadLocalFactory<Stack<ResolveStackEntry>>>(c => new ThreadLocalFactory<Stack<ResolveStackEntry>>(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())));

            _serviceProvider = _container.Resolve<IServiceProvider>();
            _beanDefinitionContainer = _container.Resolve<IBeanDefinitionContainer>();

            var loaderEntry = new BeanLoaderEntry(BeanLoaderEntryOrder.Intern, true);

            loaderEntry.ConfigureBeanDefinitions(defs =>
            {
                defs.AddFromServiceCollection(services =>
                {
                    services
                        .AddSingleton(sp => _beanDefinitionContainer)
                        .AddSingleton(sp => _serviceProvider)
                        .AddSingleton<IServiceScopeFactory>(sp => (UnityAddonServiceProvider)_serviceProvider)
                        .AddSingleton<IServiceScope>(sp => (UnityAddonServiceProvider)_serviceProvider);
                });
            });

            loaderEntry.PostProcess += container => container.AddNewExtension<BeanBuildStrategyExtension>();

            var beanConstructEntry = new BeanLoaderEntry(BeanLoaderEntryOrder.Intern, true);

            beanConstructEntry.ConfigureBeanDefinitions(defs =>
                {
                    defs.AddFromServiceCollection(services =>
                    {
                        services
                            .AddSingleton<ConstructorResolver>()
                            .AddSingleton<ParameterFill>()
                            .AddSingleton<PropertyFill>()
                            .AddSingleton<BeanFactory>();
                    });
                });

            var beanResolveEntry = new BeanLoaderEntry(BeanLoaderEntryOrder.NetAsp + 1, true);

            beanResolveEntry.ConfigureBeanDefinitions(defs =>
                        {
                            defs.AddFromServiceCollection(services =>
                            {
                                services.AddSingleton<ValueProvider>()
                                    .AddSingleton<ConfigBracketParser>()
                                    .AddSingleton<AopBuildStrategyExtension>()
                                    .AddSingleton<BeanAopStrategy>()
                                    .AddSingleton<AopMethodBootstrapInterceptor>()
                                    .AddSingleton<InterfaceProxyFactory>()
                                    .AddSingleton<ProxyGenerator>()
                                    .AddSingleton<BeanMethodInterceptor>()
                                    .AddSingleton(sp => (sp.GetService<AopInterceptorContainerBuilder>() ?? new AopInterceptorContainerBuilder()).Build(sp))
                                    .AddSingleton(sp => (sp.GetService<BeanDefintionCandidateSelectorBuilder>() ?? new BeanDefintionCandidateSelectorBuilder()).Build(sp.GetService<IConfiguration>()));
                            });
                        });

            Add(loaderEntry);
            Add(beanConstructEntry);
            Add(beanResolveEntry);
        }



        public void Add(BeanLoaderEntry entry)
        {
            _entries.Add(entry);
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

            foreach (var beanDef in loadEntry.BeanDefinitionCollection)
            {
                if (beanDef.Type == typeof(IBeanDefinition))
                {
                    child.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_serviceProvider, t, n), (IFactoryLifetimeManager)beanDef.Scope);
                }
                else
                {
                    _beanDefinitionContainer.RegisterBeanDefinition(beanDef);
                    _container.RegisterFactory(beanDef.Type, beanDef.Name, (c, t, n) => beanDef.Constructor(_serviceProvider, t, n), (IFactoryLifetimeManager)beanDef.Scope);
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
