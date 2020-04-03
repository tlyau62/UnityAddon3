using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanBuildStrategies;

namespace UnityAddon.Core
{
    public static class UnityAddonHost
    {
        public static IHost InitUnityAddon(this IHost host)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.AddNewExtension<BeanBuildStrategyExtension>();

            var scanner = container.Resolve<ComponentScanner>();
            var configParser = container.Resolve<ConfigurationParser>();

            scanner.ScanComponent(Assembly.GetExecutingAssembly(), container, "UnityAddon.Core");
            configParser.ParseScannedConfigurations(container);

            return host;
        }

        public static IHost ScanComponentUnityAddon(this IHost host, Assembly assembly, params string[] namespaces)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
            var scanner = container.Resolve<ComponentScanner>();
            var configParser = container.Resolve<ConfigurationParser>();

            scanner.ScanComponent(assembly, container, namespaces);
            configParser.ParseScannedConfigurations(container);

            return host;
        }

        public static IHost ScanComponentUnityAddon(this IHost host, params string[] namespaces)
        {
            return host.ScanComponentUnityAddon(Assembly.GetCallingAssembly(), namespaces);
        }

        public static IHost EnableTestMode(this IHost host, object testobject)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.BuildUp(testobject.GetType(), testobject);

            return host;
        }
    }
}
