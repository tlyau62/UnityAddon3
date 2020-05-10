using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Hangfire
{
    public static class UnityAddonHangfireHostBuilder
    {
        public static IHostBuilder EnableUnityAddonHangfire(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureContext<DependencyResolverOption>(option =>
                    {
                        option.AddResolveStrategy<HangfireProxyAttribute>((t, n, sp) =>
                        {
                            return sp.GetRequiredService(t, t.Name);
                        });
                    });
                    ctx.ConfigureContext<AopInterceptorContainerOption>(option =>
                    {
                        option.AddAopIntercetor<HangfireProxyInterceptor>();
                    });
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire"));
                });
        }
    }
}
