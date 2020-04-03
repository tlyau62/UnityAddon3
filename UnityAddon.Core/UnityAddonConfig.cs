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
        public static IHostBuilder RegisterUnityAddon(this IHostBuilder hostBuilder, IUnityContainer container = null)
        {
            if (container == null)
            {
                container = new UnityContainer();
                hostBuilder.Properties["_UnityAddon_IsNewContainer"] = true;
            }
            else
            {
                hostBuilder.Properties["_UnityAddon_IsNewContainer"] = false;
            }

            container.RegisterType<IBeanDefinitionContainer, BeanDefinitionContainer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IThreadLocalFactory<Stack<IInvocation>>, ThreadLocalFactory<Stack<IInvocation>>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new Func<Stack<IInvocation>>(() => new Stack<IInvocation>())));

            var simpleDefs = new[] {
                typeof(IThreadLocalFactory<Stack<IInvocation>>),
                typeof(IUnityContainer),
                typeof(ProxyGenerator),
                typeof(ParameterFill),
                typeof(DependencyResolver),
                typeof(ConfigurationFactory),
                typeof(BeanFactory),
                typeof(ComponentScanner)
            };
            var defContainer = container.Resolve<IBeanDefinitionContainer>();

            foreach (var def in simpleDefs)
            {
                defContainer.RegisterBeanDefinition(new SimpleBeanDefinition(def));
            }

            return hostBuilder.UseUnityServiceProvider(container);
        }

        public static IHostBuilder MergeUnityAddon(this IHostBuilder hostBuilder)
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
                    if ((bool)hostBuilder.Properties["_UnityAddon_IsNewContainer"])
                    {
                        c.Resolve<IHostApplicationLifetime>().ApplicationStopped.Register(() => c.Dispose());
                    }
                });
        }

    }
}
