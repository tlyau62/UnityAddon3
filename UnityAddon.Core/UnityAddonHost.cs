using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;

namespace UnityAddon.Core
{
    public static class UnityAddonHost
    {
        public static IHost ScanComponentsUA(this IHost host, Assembly assembly, params string[] namespaces)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
            var beanDefContainer = container.Resolve<IBeanDefinitionContainer>();
            var beanFactory = container.Resolve<BeanFactory>();
            var scanner = container.Resolve<ComponentScanner>();
            var beanDefs = scanner.ScanComponents(assembly, namespaces);

            beanDefContainer.RegisterBeanDefinitions(beanDefs);
            beanFactory.CreateFactory(beanDefs, container);

            return host;
        }

        public static IHost ScanComponentsUA(this IHost host, params string[] namespaces)
        {
            return host.ScanComponentsUA(Assembly.GetCallingAssembly(), namespaces);
        }

        public static IHost RunTestUA(this IHost host, object testobject)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.BuildUp(testobject.GetType(), testobject);

            return host;
        }

        public static IHost PreInstantiateSingleton(this IHost host)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.PreInstantiateSingleton();

            return host;
        }
    }
}
