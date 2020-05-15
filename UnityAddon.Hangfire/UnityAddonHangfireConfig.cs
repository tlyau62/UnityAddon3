using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
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
        [Bean]
        public virtual DependencyResolverOption DependencyResolverOption()
        {
            var option = new DependencyResolverOption();

            option.AddResolveStrategy<HangfireProxyAttribute>((t, n, sp) =>
            {
                return sp.GetRequiredService(t, t.Name);
            });

            return option;
        }

        [Bean]
        public virtual AopInterceptorOption AopInterceptorOption()
        {
            var option = new AopInterceptorOption();

            option.AddAopIntercetor<HangfireProxyInterceptor>();

            return option;
        }

        [Bean]
        public virtual IBeanDefinitionCollection EnableUnityAddonHangfire()
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire");

            return col;
        }
    }

    public static class UnityAddonHangfireExt
    {
        public static void AddUnityAddonHangfire(this IBeanRegistry beanRegistry)
        {
            beanRegistry.AddConfiguration<UnityAddonHangfireConfig>();
        }
    }
}
