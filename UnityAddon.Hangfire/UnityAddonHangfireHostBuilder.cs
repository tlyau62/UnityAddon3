using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Component;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Hangfire
{
    public static class UnityAddonHangfireHostBuilder
    {
        public static IHostBuilder EnableUnityAddonHangfire(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureUA<ComponentScannerBuilder>(c =>
                {
                    c.AddComponentScannerStrategy<HangfireComponentScannerStrategy>();
                })
                .ConfigureUA<DependencyResolverBuilder>(c =>
                    c.AddResolveStrategy<HangfireProxyAttribute>((t, n, c) =>
                    {
                        return c.ResolveUA(t, $"{t.Name}HangfireProxy");
                    })
                )
                .ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");
        }
    }
}
