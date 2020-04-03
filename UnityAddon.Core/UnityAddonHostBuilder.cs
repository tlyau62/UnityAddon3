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

namespace UnityAddon.Core
{
    public static class UnityAddonHostBuilder
    {
        public static IHostBuilder RegisterUA(this IHostBuilder hostBuilder, Action<IConfigurationBuilder> config = null, IUnityContainer container = null)
        {
            config ??= (conf) => { };

            if (container == null)
            {
                container = new UnityContainer();
                hostBuilder.Properties["_UnityAddon_IsNewContainer"] = true;
            }
            else
            {
                hostBuilder.Properties["_UnityAddon_IsNewContainer"] = false;
            }

            container
                .RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager())
                .RegisterType<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())))
                .RegisterInstance(BuildConfig()); // add config instance

            var scanner = container.Resolve<ComponentScanner>();
            var ext = container.Resolve<BeanBuildStrategyExtension>();
            container.AddExtension(ext);

            return hostBuilder.UseUnityServiceProvider(container)
                .ConfigureAppConfiguration((ctx, conf) => config(conf)) // add config bean def
                .ConfigureContainer<IUnityContainer>(c =>
                {
                    // refresh
                    scanner.ScanComponent(Assembly.GetExecutingAssembly(), c, "UnityAddon.Core");

                    // add non-component dep def
                    c.Resolve<IBeanDefinitionContainer>()
                        .RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IThreadLocalFactory<Stack<IInvocation>>)))
                        .RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IUnityContainer)));

                    c.Resolve<BeanBuildStrategyExtension>();

                    c.BuildUp(ext);
                });

            IConfiguration BuildConfig()
            {
                var configBuilder = new ConfigurationBuilder();

                config(configBuilder);

                return configBuilder.Build();
            }
        }

        public static IHostBuilder ScanComponentUA(this IHostBuilder hostBuilder, Assembly assembly, params string[] namespaces)
        {
            return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    var scanner = c.Resolve<IComponentScanner>();

                    scanner.ScanComponent(assembly, c, namespaces);
                });
        }

        public static IHostBuilder ScanComponentUA(this IHostBuilder hostBuilder, params string[] namespaces)
        {
            return hostBuilder.ScanComponentUA(Assembly.GetCallingAssembly(), namespaces);
        }

        public static IHostBuilder EnableTestMode(this IHostBuilder hostBuilder, UnityAddonTest testobject)
        {
            return hostBuilder
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    c.BuildUp(testobject.GetType(), testobject);
                });
        }

        public static IHostBuilder InitUA(this IHostBuilder hostBuilder)
        {
            IList<IBeanDefinition> defs = new List<IBeanDefinition>();

            if (hostBuilder.Properties.ContainsKey("_UnityAddon_IsInitialized") && (bool)hostBuilder.Properties["_UnityAddon_IsInitialized"])
            {
                throw new InvalidOperationException("Already isInitialized");
            }
            else
            {
                hostBuilder.Properties["_UnityAddon_IsInitialized"] = true;
            }

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
                    var defCon = c.Resolve<IBeanDefinitionContainer>();

                    defCon.RegisterBeanDefinitions(defs);
                })
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    var configParser = c.Resolve<ConfigurationParser>();

                    // parse config
                    configParser.ParseScannedConfigurations(c);
                })
                .ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    if ((bool)hostBuilder.Properties["_UnityAddon_IsNewContainer"])
                    {
                        c.Resolve<IHostApplicationLifetime>().ApplicationStopped.Register(() => c.Dispose());
                    }
                });
        }

    }
}
