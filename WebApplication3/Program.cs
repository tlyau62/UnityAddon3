using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;

namespace WebApplication3
{
    public class LifetimeEventsHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;

        public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).BuildUA().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .RegisterUA()
                .ScanComponentsUA("WebApplication3")
                .ConfigureServices(c =>
                {
                    c.AddTransient<IHostedService, LifetimeEventsHostedService>((x) =>
                    {
                        var container = x.GetService<IUnityContainer>();

                        return container.Resolve<LifetimeEventsHostedService>();
                    });

                    // c.AddTransient<IHostedService, LifetimeEventsHostedService>();

                    //public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
                });
    }
}
