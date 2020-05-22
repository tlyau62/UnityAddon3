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
    [Configuration]
    [IgnoreDuringScan]
    public class UnityAddonCacheConfig
    {
        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

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

        [Bean]
        public virtual AopInterceptorOption AopInterceptorOption()
        {
            var option = new AopInterceptorOption();

            option
                .AddAopIntercetor<CacheInterceptor>()
                .AddAopIntercetor<InvalidateCacheInterceptor>();

            return option;
        }
    }
}
