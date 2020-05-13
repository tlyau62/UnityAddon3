using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Hangfire
{
    public class UnityAddonHangfireConfig
    {
        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [PostConstruct]
        public void Init()
        {
            ApplicationContext.ConfigureContext<DependencyResolverOption>(option =>
            {
                option.AddResolveStrategy<HangfireProxyAttribute>((t, n, sp) =>
                {
                    return sp.GetRequiredService(t, t.Name);
                });
            });

            ApplicationContext.ConfigureContext<AopInterceptorContainerOption>(option =>
            {
                option.AddAopIntercetor<HangfireProxyInterceptor>();
            });
        }

        [Bean]
        public virtual IBeanDefinitionCollection EnableUnityAddonHangfire()
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");

            return col;
        }
    }
}
