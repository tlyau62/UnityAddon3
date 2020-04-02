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
    public static class UnityAddonConfig
    {
        private static bool _isInited = false;

        public static IHostBuilder RegisterUnityAddon(this IHostBuilder hostBuilder)
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));

            var scanner = container.Resolve<ComponentScanner>();

            container.AddNewExtension<BeanBuildStrategyExtension>();

            // refresh
            scanner.ScanComponent(Assembly.GetExecutingAssembly(), container, "UnityAddon.Core");

            // add non-component dep def
            container.Resolve<IBeanDefinitionContainer>()
                .RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IThreadLocalFactory<Stack<IInvocation>>)));

            return hostBuilder.UseUnityServiceProvider(container);
        }

        public static IHostBuilder ScanComponentUnityAddon(this IHostBuilder hostBuilder, Assembly assembly, params string[] namespaces)
        {
            return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
                {
                    var scanner = c.Resolve<IComponentScanner>();

                    scanner.ScanComponent(assembly, c, namespaces);
                });
        }

        public static IHostBuilder ScanComponentUnityAddon(this IHostBuilder hostBuilder, params string[] namespaces)
        {
            return hostBuilder.ScanComponentUnityAddon(Assembly.GetCallingAssembly(), namespaces);
        }

        public static IHostBuilder InitUnityAddon(this IHostBuilder hostBuilder)
        {
            IList<IBeanDefinition> defs = new List<IBeanDefinition>();

            if (_isInited)
            {
                throw new InvalidOperationException("Already inited");
            }
            else
            {
                _isInited = true;
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
                });
        }

        public static IHostBuilder PreInstantiateSingletonUnityAddon(this IHostBuilder hostBuilder)
        {
            return hostBuilder
               .ConfigureContainer<IUnityContainer>((s, c) =>
               {
                   PreInstantiateSingleton(c);
               });
        }

        /// <summary>
        /// Instantiate singleton bean recursively.
        /// Some bean may do bean registration at postconstruct,
        /// so recursive needed.
        /// 
        /// The final number of un-registrations will be converge to 0,
        /// since each bean is postconstructed once only.
        /// </summary>
        private static void PreInstantiateSingleton(IUnityContainer container)
        {
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations)
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager))
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {
                    container.Resolve(reg.RegisteredType, reg.Name);
                }
            }

            if (container.Registrations.Count() != currentRegs)
            {
                PreInstantiateSingleton(container);
            }
        }
    }
}
