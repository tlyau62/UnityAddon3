using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Core
{
    public static class UnityAddonServiceCollection
    {
        public static IServiceCollection AddHostedServiceUA<THostedService>(this IServiceCollection services)
            where THostedService : class, IHostedService
            => services.AddTransient<IHostedService, THostedService>(s =>
            {
                var container = s.GetService<IUnityContainer>();

                return container.Resolve<THostedService>();
            });
    }
}
