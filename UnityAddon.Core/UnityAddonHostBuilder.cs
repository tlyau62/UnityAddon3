using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;
using UnityAddon.Core.Bean;
using Microsoft.Extensions.DependencyInjection;
using Unity.Microsoft.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using System.Reflection;
using UnityAddon.Core.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using Castle.DynamicProxy;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Component;
using UnityAddon.Core.Aop;

namespace UnityAddon.Core
{
    public static class UnityAddonHostBuilder
    {
        public static IHostBuilder RegisterUA(this IHostBuilder hostBuilder, IUnityContainer container = null)
        {
            var isNewContainer = container == null;

            container ??= new UnityContainer();

            return hostBuilder.UseUnityServiceProvider(container)
                .ConfigureContainer<IUnityContainer>(c =>
                {
                    var beanDefCol = new BeanDefinitionCollection("CORE");

                    beanDefCol.Add(new SimpleBeanDefinition(typeof(IUnityContainer)));

                    c.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                     .RegisterInstanceUA((IBeanDefinitionCollection)beanDefCol, "core")
                     .RegisterInstanceUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>(new List<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>());
                })
                .ScanComponentsUA("UnityAddon.Core")
                .MergeFromServiceCollectionUA()
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    if (isNewContainer)
                    {
                        c.Resolve<IHostApplicationLifetime>().ApplicationStopped.Register(() => c.Dispose());
                    }
                });
        }

        public static IHostBuilder ScanComponentsUA(this IHostBuilder hostBuilder, Assembly assembly, params string[] namespaces)
        {
            return hostBuilder.ScanComponentsUA(assembly, (b) => { }, namespaces);
        }

        public static IHostBuilder ScanComponentsUA(this IHostBuilder hostBuilder, Assembly assembly, Action<ComponentScannerBuilder> config, params string[] namespaces)
        {
            return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
            {
                var callbacks = c.ResolveUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>();

                callbacks.Add(cs => cs.ScanComponents(assembly, namespaces));
            });
        }

        public static IHostBuilder ScanComponentsUA(this IHostBuilder hostBuilder, params string[] namespaces)
        {
            return hostBuilder.ScanComponentsUA(Assembly.GetCallingAssembly(), namespaces);
        }

        private static IHostBuilder MergeFromServiceCollectionUA(this IHostBuilder hostBuilder)
        {
            IList<IBeanDefinition> defs = new List<IBeanDefinition>();

            return hostBuilder
                .ConfigureServices((c, s) =>
                {
                    foreach (var descriptor in s)
                    {
                        defs.Add(new SimpleBeanDefinition(descriptor.ServiceType));
                    }
                })
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    var defCollection = c.ResolveUA<IBeanDefinitionCollection>("core");

                    ((BeanDefinitionCollection)defCollection).AddRange(defs);
                });
        }

        public static IHostBuilder ConfigureUA<ConfigT>(this IHostBuilder hostBuilder, Action<ConfigT> config)
        {
            return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
            {
                if (!c.IsRegisteredUA<ConfigT>())
                {
                    c.RegisterTypeUA<ConfigT, ConfigT>(new ContainerControlledLifetimeManager());
                }

                config(c.ResolveUA<ConfigT>());
            });
        }

        public static IHost BuildUA(this IHostBuilder hostBuilder)
        {
            var host = hostBuilder.Build();
            var hostContainer = host.Services.GetService<IUnityContainer>();
            var config = hostContainer.Resolve<IConfiguration>();
            var beanDefFilters = hostContainer
                .Resolve<BeanDefintionCandidateSelectorBuilder>().Build(config);
            var compScanner = hostContainer.ResolveUA<ComponentScannerBuilder>().Build(hostContainer);
            var compScannedDefs = hostContainer.ResolveUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>().SelectMany(cb => cb(compScanner));
            var beanDefCollection = hostContainer.Resolve<IBeanDefinitionCollection>("core")
                .Union(compScannedDefs)
                .Where(d => !d.FromComponentScanning || beanDefFilters.Filter(d));
            var configParser = new ConfigurationParser();

            beanDefCollection = beanDefCollection.Union(configParser.Parse(beanDefCollection));

            hostContainer
                .Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinitions(beanDefCollection);

            hostContainer
                .RegisterTypeUA<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())))
                .RegisterTypeUA<IThreadLocalFactory<Stack<ResolveStackEntry>>, ThreadLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())))
                .RegisterInstanceUA(hostContainer.Resolve<DependencyResolverBuilder>().Build())
                .RegisterInstanceUA(hostContainer.Resolve<AopInterceptorContainerBuilder>().Build(hostContainer))
                .AddNewExtension<BeanBuildStrategyExtension>();

            hostContainer
                .ResolveUA<BeanFactory>()
                .CreateFactory(beanDefCollection, hostContainer);

            foreach (var defCollection in hostContainer.ResolveAllUA<IBeanDefinitionCollection>().Where(c => c.Name != "CORE"))
            {
                hostContainer
                    .Resolve<IBeanDefinitionContainer>()
                    .RegisterBeanDefinitions(defCollection);

                hostContainer
                    .ResolveUA<BeanFactory>()
                    .CreateFactory(defCollection, hostContainer);
            }

            return host;
        }
    }
}
