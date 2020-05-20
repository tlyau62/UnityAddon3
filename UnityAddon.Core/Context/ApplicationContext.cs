using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Reflection;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Context
{
    public class ApplicationContext
    {
        [Dependency]
        public IUnityAddonSP ApplicationSP { get; set; }

        [Dependency]
        public ConfigurationRegistry ConfigurationRegistry { get; set; }

        [Dependency]
        public BeanDefinitionRegistry BeanDefinitionRegistry { get; set; }

        [Dependency]
        public BeanBuildStrategyExtension BeanBuildStrategyExtension { get; set; }

        [Dependency]
        public AopBuildStrategyExtension AopBuildStrategyExtension { get; set; }

        [Dependency]
        public IServiceRegistry ServiceRegistry { get; set; }

        [Dependency]
        public ImportRegistry ImportRegistry { get; set; }

        public IUnityAddonSP Build()
        {
            Config();
            PostRegistry();
            PostRegistryInit();
            PreInstantiateSingleton();
            PostInstantiateSingleton();

            return ApplicationSP;
        }

        public void Config()
        {
            ApplicationSP.UnityContainer.AddExtension(BeanBuildStrategyExtension);
            ApplicationSP.UnityContainer.AddExtension(AopBuildStrategyExtension);
        }

        public void PostRegistryInit()
        {
            ApplicationSP.GetServices<IContextPostRegistryInitiable>().ToList().ForEach(init => init.Initialize());
        }

        public void PostInstantiateSingleton()
        {
            ApplicationSP.GetServices<IContextPostInstantiateSingleton>().ToList().ForEach(init => init.PostInitialize());
        }

        public void PostRegistry(int? count = null)
        {
            if (count == null)
            {
                count = 0;
            }

            var newCount = ApplicationSP.UnityContainer.Registrations.Count();

            if (count == newCount)
            {
                return;
            }

            count = newCount;
            ConfigurationRegistry.Refresh();
            BeanDefinitionRegistry.Refresh();
            ImportRegistry.Refresh();
            PostRegistry(count);
        }

        public void PreInstantiateSingleton()
        {
            var sp = ApplicationSP;
            var container = ApplicationSP.UnityContainer;
            var currentRegs = container.Registrations.Count();

            foreach (var reg in container.Registrations.ToArray())
            {
                if (!(reg.LifetimeManager is ContainerControlledLifetimeManager) || reg.Name.EndsWith("factory")) // skip bean method factory bean
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
                PreInstantiateSingleton();
            }
        }
    }
}
