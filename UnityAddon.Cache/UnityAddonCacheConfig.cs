using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Cache
{
    public class UnityAddonCacheConfig
    {
        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [PostConstruct]
        public void Setup()
        {
            ApplicationContext.ConfigureContext<AopInterceptorContainerOption>(option =>
            {
                option
                    .AddAopIntercetor<CacheInterceptor>()
                    .AddAopIntercetor<InvalidateCacheInterceptor>();
            });
        }

        [Bean]
        public virtual IBeanDefinitionCollection EnableUnityAddonCache()
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Cache");
            col.AddFromServiceCollection(config =>
            {
                config.AddMemoryCache();
            });

            return col;
        }
    }
}
