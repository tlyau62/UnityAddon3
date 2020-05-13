﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Ef.Transaction;
using Unity.Lifetime;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Bean.Config;

namespace UnityAddon.Ef
{
    public class UnityAddonEfConfig
    {
        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [PostConstruct]
        public void Setup()
        {
            ApplicationContext.ConfigureContext<AopInterceptorContainerOption>(config =>
            {
                config.AddAopIntercetor<RequireDbContextInterceptor>()
                    .AddAopIntercetor<RepositoryInterceptor>();
            });
        }

        [Bean]
        public virtual IBeanDefinitionCollection UnityAddonEfBeans()
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Ef");

            return col;
        }
    }
}
