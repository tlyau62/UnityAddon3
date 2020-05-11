using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Ef.Transaction;
using Unity.Lifetime;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Ef
{
    public static class UnityAddonEfHostBuilder
    {
        public static IHostBuilder EnableUnityAddonEf(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Ef"));
                    ctx.ConfigureContext<AopInterceptorContainerOption>(config =>
                    {
                        config
                            .AddAopIntercetor<RequireDbContextInterceptor>()
                            .AddAopIntercetor<RepositoryInterceptor>();
                    });
                });
        }
    }
}
