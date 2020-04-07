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

namespace UnityAddon.Ef
{
    public static class UnityAddonEfHostBuilder
    {
        public static IHostBuilder EnableUnityAddonEf(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Ef")
                .ConfigureUA<AopInterceptorContainerBuilder>(config =>
                {
                    config
                        .AddAopIntercetor<RequireDbContextInterceptor>()
                        .AddAopIntercetor<RepositoryInterceptor>();
                })
                .ConfigureContainer<IUnityContainer>(container =>
                {
                    container.RegisterTypeUA<TransactionCallbacks, TransactionCallbacks>(new ContainerControlledLifetimeManager())
                     .RegisterInstanceUA<ITransactionCallbacks>(container.ResolveUA<TransactionCallbacks>());

                    container.RegisterFactoryUA((c, t, n) => c.Resolve<DbContextTemplateBuilder>().Build(c));
                });
        }

    }
}
