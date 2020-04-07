using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core;

namespace UnityAddon.Hangfire
{
    public static class UnityAddonHangfireHostBuilder
    {
        public static IHostBuilder ScanComponentsUAHangfire(this IHostBuilder hostBuilder, Assembly assembly, params string[] namespaces)
        {
            return hostBuilder.ScanComponentsUA(assembly, b => b.AddComponentScannerStrategy<HangfireComponentScannerStrategy>(), namespaces);
        }

        public static IHostBuilder EnableUnityAddonHangfireEf(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");
        }
    }
}
