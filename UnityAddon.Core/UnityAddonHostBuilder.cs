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
using Unity.Strategies;
using Unity.Builder;

namespace UnityAddon.Core
{
    public static class UnityAddonHostBuilder
    {
        private static readonly string IS_NEW_CONTAINER = "__IS_NEW_CONTAINER";

        public static IHostBuilder RegisterUA(this IHostBuilder hostBuilder, IUnityContainer container = null)
        {
            hostBuilder.Properties[IS_NEW_CONTAINER] = container == null;

            container ??= new UnityContainer();

            container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .RegisterTypeUA<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())))
                .RegisterTypeUA<IThreadLocalFactory<Stack<ResolveStackEntry>>, ThreadLocalFactory<Stack<ResolveStackEntry>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<ResolveStackEntry>>(() => new Stack<ResolveStackEntry>())))
                .RegisterTypeUA<DependencyResolver, DependencyResolver>(new ContainerControlledLifetimeManager())
                .AddNewExtension<BeanBuildStrategyExtension>();

            return hostBuilder.UseUnityServiceProvider(container)
                .ConfigureContainer<IUnityContainer>(c =>
                {
                    c.RegisterInstanceUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>(new List<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>());
                })
                .ScanComponentsUA("UnityAddon.Core")
                .MergeFromServiceCollectionUA();
        }

        public static IHostBuilder ScanComponentsUA(this IHostBuilder hostBuilder, Assembly assembly, params string[] namespaces)
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
            IServiceCollection serviceCollection = new ServiceCollection();

            return hostBuilder
                .ConfigureServices((c, s) =>
                {
                    serviceCollection = s;
                })
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    var callbacks = c.ResolveUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>();

                    callbacks.Add(cs => cs.ScanComponents(serviceCollection));
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
            var compScanner = hostContainer.Resolve<ComponentScannerBuilder>().Build(hostContainer);
            var compScannedDefs = hostContainer.ResolveUA<IList<Func<ComponentScanner, IEnumerable<IBeanDefinition>>>>().SelectMany(cb => cb(compScanner));
            var beanDefCollection = compScannedDefs.Where(d => !d.FromComponentScanning || beanDefFilters.Filter(d));
            var configParser = new ConfigurationParser();

            beanDefCollection = beanDefCollection.Union(configParser.Parse(beanDefCollection));

            hostContainer
                .Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinitions(beanDefCollection);

            hostContainer.RegisterInstanceUA(hostContainer
                .Resolve<AopInterceptorContainerBuilder>().Build(hostContainer));
            hostContainer.AddNewExtension<AopBuildStrategyExtension>();

            hostContainer
                .ResolveUA<BeanFactory>()
                .CreateFactory(beanDefCollection, hostContainer);

            foreach (var defCollection in hostContainer.ResolveAllUA<IBeanDefinitionCollection>())
            {
                hostContainer
                    .Resolve<IBeanDefinitionContainer>()
                    .RegisterBeanDefinitions(defCollection);

                hostContainer
                    .ResolveUA<BeanFactory>()
                    .CreateFactory(defCollection, hostContainer);
            }

            if ((bool)hostBuilder.Properties[IS_NEW_CONTAINER])
            {
                hostContainer
                    .Resolve<IHostApplicationLifetime>()
                    .ApplicationStopped.Register(() => hostContainer.Dispose());
            }

            return host;
        }
    }
}
