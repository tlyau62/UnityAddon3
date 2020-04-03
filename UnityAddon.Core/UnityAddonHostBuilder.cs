﻿using Microsoft.Extensions.Hosting;
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
                    c.RegisterType<IBeanDefinitionCollection, BeanDefinitionCollection>(new ContainerControlledLifetimeManager());
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
            return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
            {
                var defCollection = c.Resolve<IBeanDefinitionCollection>();
                var scanner = c.Resolve<ComponentScanner>();
                var defs = scanner.ScanComponents(assembly, namespaces);

                ((BeanDefinitionCollection)defCollection).AddRange(defs);
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
                    var defCollection = c.Resolve<IBeanDefinitionCollection>();

                    ((BeanDefinitionCollection)defCollection).AddRange(defs);
                });
        }

        public static IHost BuildUA(this IHostBuilder hostBuilder)
        {
            var host = hostBuilder.Build();
            var hostContainer = host.Services.GetService<IUnityContainer>();
            var beanDefCollection = hostContainer.Resolve<BeanDefintionCandidateSelector>()
                .Select(hostContainer.Resolve<IBeanDefinitionCollection>());

            hostContainer
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinitions(beanDefCollection);

            hostContainer
                .RegisterType<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())))
                .RegisterType<BeanFactory>(new ContainerControlledLifetimeManager())
                .AddNewExtension<BeanBuildStrategyExtension>();

            hostContainer
                .Resolve<BeanFactory>()
                .CreateFactory(beanDefCollection, hostContainer);

            return host;
        }
    }
}
