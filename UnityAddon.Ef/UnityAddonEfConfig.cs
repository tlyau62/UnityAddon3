using Microsoft.Extensions.Hosting;
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
using UnityAddon.Ef.RollbackLogics;
using UnityAddon.Ef.TransactionInterceptor;

namespace UnityAddon.Ef
{
    [Configuration]
    [IgnoreDuringScan]
    public class UnityAddonEfConfig : AopInterceptorConfig
    {
        [Bean]
        public override AopInterceptorOption AopInterceptorOption()
        {
            var option = new AopInterceptorOption();

            option
                .AddAopIntercetor<RequireDbContextInterceptor>()
                .AddAopIntercetor<RepositoryInterceptor>();

            return option;
        }

        [Bean]
        public virtual IBeanDefinitionCollection UnityAddonEfBeans()
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(Assembly.GetExecutingAssembly(), "UnityAddon.Ef");

            return col;
        }
    }

    public static class UnityAddonEfExt
    {
        public static void AddUnityAddonEf(this IBeanRegistry beanRegistry)
        {
            beanRegistry.AddConfiguration<UnityAddonEfConfig>();
        }
    }
}
