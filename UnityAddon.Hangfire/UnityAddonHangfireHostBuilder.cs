using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core;

namespace UnityAddon.Hangfire
{
    public class UnityAddonHangfireHostBuilder
    {
        public static IHostBuilder EnableUnityAddonHangfire(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ScanComponentsUA(Assembly.GetExecutingAssembly(), "UnityAddon.Hangfire")
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
