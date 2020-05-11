using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Cache
{
    public static class UnityAddonCacheHostBuilder
    {
        public static IHostBuilder EnableUnityAddonCache(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Cache"));
                    ctx.ConfigureContext<AopInterceptorContainerOption>(config =>
                    {
                        config
                            .AddAopIntercetor<CacheInterceptor>()
                            .AddAopIntercetor<InvalidateCacheInterceptor>();
                    });
                });
        }
    }
}
