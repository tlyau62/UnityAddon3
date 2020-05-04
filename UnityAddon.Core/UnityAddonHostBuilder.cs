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
using UnityAddon.Core.BeanDefinition;
using Castle.DynamicProxy;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Aop;
using Unity.Strategies;
using Unity.Builder;

namespace UnityAddon.Core
{
    public static class UnityAddonHostBuilder
    {
        private static readonly string IS_NEW_CONTAINER = "__IS_NEW_CONTAINER";

        public static IHostBuilder RegisterUA(this IHostBuilder hostBuilder, IUnityContainer container)
        {
            return hostBuilder.UseServiceProviderFactory(new ServiceProviderFactory(container));
        }

        public static IHostBuilder RegisterUA(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseServiceProviderFactory(new ServiceProviderFactory());
        }

        //public static IHostBuilder ConfigureUA<ConfigT>(this IHostBuilder hostBuilder, Action<ConfigT> config)
        //{
        //    return hostBuilder.ConfigureContainer<IUnityContainer>((s, c) =>
        //    {
        //        var sp = c.Resolve<IServiceProvider>();

        //        if (!sp.IsRegistered<ConfigT>())
        //        {
        //            c.RegisterTypeUA<ConfigT, ConfigT>(new ContainerControlledLifetimeManager());
        //        }

        //        config(c.ResolveUA<ConfigT>());
        //    });
        //}
    }
}
