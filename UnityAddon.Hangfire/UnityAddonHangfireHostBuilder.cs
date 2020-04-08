using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Component;

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
                .ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");
        }
    }
}
