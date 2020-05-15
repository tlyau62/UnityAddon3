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
        private readonly IUnityContainer _container;

        public ServiceProviderFactory() : this(new UnityContainer())
        {
        }

        public ServiceProviderFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ApplicationContext CreateBuilder(IServiceCollection services = null)
        {
            var appCtx = new ApplicationContext(_container);

            // asp core
            appCtx.ConfigureBeans(config => config.AddFromServiceCollection(services));

            // value provider
            appCtx.ConfigureBeans(config => config.AddConfiguration<ValueConfig>());

            return appCtx;
        }

        public IServiceProvider CreateServiceProvider(ApplicationContext appCtx)
        {
            return appCtx.Build();
        }
    }

}


