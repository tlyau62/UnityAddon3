﻿using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanBuildStrategies;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Thread;
using UnityAddon.Core.Value;

namespace UnityAddon.Core
{
    public class ServiceProviderFactory : IServiceProviderFactory<ApplicationContext>
    {
        public ApplicationContext CreateBuilder(IServiceCollection services = null)
        {
            var coreCtx = new CoreContext();
            var appCtx = coreCtx.Container.Resolve<ApplicationContext>();
            var beanReg = appCtx.ServiceRegistry;

            ConfigAppContainer(appCtx.ApplicationSP.UnityContainer, coreCtx.Container);

            beanReg.ConfigureBeans(config => config.AddFromUnityContainer(coreCtx.Container));
            beanReg.ConfigureBeans(config => config.AddFromUnityContainer(new ValueConfig().Build(appCtx.ApplicationSP)));

            if (services != null)
            {
                beanReg.ConfigureBeans(config => config.AddFromServiceCollection(services));
            }

            return appCtx;
        }

        public IServiceProvider CreateServiceProvider(ApplicationContext appCtx)
        {
            return appCtx.Build();
        }

        public void ConfigAppContainer(IUnityContainer appContainer, IUnityContainer coreContainer)
        {
            appContainer.AddExtension(coreContainer.Resolve<BeanBuildStrategyExtension>());
            appContainer.AddExtension(coreContainer.Resolve<AopBuildStrategyExtension>());
        }
    }

}



