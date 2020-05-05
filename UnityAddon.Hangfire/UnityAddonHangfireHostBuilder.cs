using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Component;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Hangfire
{
    public static class UnityAddonHangfireHostBuilder
    {
        public static IHostBuilder EnableUnityAddonHangfire(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureUA<DependencyResolver>(c =>
                    c.AddResolveStrategy<HangfireProxyAttribute>((t, n, c) =>
                    {
                        return c.ResolveUA(t, t.Name);
                    })
                )
                .ConfigureUA<AopInterceptorContainerOption>(c =>
                {
                    c.AddAopIntercetor<HangfireProxyInterceptor>();
                })
                .ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");
        }
    }
}
