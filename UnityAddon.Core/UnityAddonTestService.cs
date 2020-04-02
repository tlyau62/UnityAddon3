using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Core
{
    public interface IUnityAddonTest { }

    public class UnityAddonTestService : IHostedService
    {
        private IHostApplicationLifetime _appLifetime;

        public UnityAddonTestService(IUnityContainer container, IUnityAddonTest test, IHostApplicationLifetime appLifetime)
        {
            container.BuildUp(test.GetType(), test);

            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.StopApplication();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
