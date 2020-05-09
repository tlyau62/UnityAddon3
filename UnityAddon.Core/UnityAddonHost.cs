using Microsoft.Extensions.DependencyInjection;
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
using UnityAddon.Core.Context;

namespace UnityAddon.Core
{
    public static class UnityAddonHost
    {
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
            var container = host.Services.GetRequiredService<ApplicationContext>().AppContainer;
            var sp = new UnityAddonSP(container);
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations)
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager || reg.LifetimeManager is SingletonLifetimeManager))
                {
                    continue;
                }

                if (!reg.RegisteredType.IsGenericType || !reg.RegisteredType.ContainsGenericParameters)
                {
                    sp.GetRequiredService(reg.RegisteredType, reg.Name);
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
