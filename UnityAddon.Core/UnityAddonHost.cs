using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Component;

namespace UnityAddon.Core
{
    public static class UnityAddonHost
    {
        public static IHost BuildTestUA(this IHost host, object testobject)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;

            container.BuildUp(testobject.GetType(), testobject);

            return host;
        }

        /// <summary>
        /// Instantiate singleton bean recursively.
        /// Some bean may do bean registration at postconstruct,
        /// so recursive needed.
        /// 
        /// The final number of un-registrations will be converge to 0,
        /// since each bean is postconstructed once only.
        /// </summary>
        public static IHost PreInstantiateSingleton(this IHost host)
        {
            var container = host.Services.GetService(typeof(IUnityContainer)) as IUnityContainer;
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations)
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager))
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {
                    container.ResolveAllUA(reg.RegisteredType).ToList();
                }
            }

            if (container.Registrations.Count() != currentRegs)
            {
                host.PreInstantiateSingleton();
            }

            return host;
        }

    }
}
