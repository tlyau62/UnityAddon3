using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.HostService
{
    public class LifetimeEventsHostedService : IHostedService
    {
        [Value("TestValue")]
        public string Test { get; set; }

        [Dependency]
        public IHostApplicationLifetime AppLifetime { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Assert.Equal("TestValue", Test);

            Assert.NotNull(AppLifetime);

            return Task.Run(() => AppLifetime.StopApplication());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    [Trait("HostService", "HostService")]
    public class HostServiceTests
    {
        private IHost _host;

        public HostServiceTests()
        {
            _host = new HostBuilder()
                .RegisterUA()
                .ConfigureServices(c => c.AddHostedService<LifetimeEventsHostedService>())
                .BuildUA();
        }

        [Fact]
        public void UnityAddonServiceCollection_AddHostedService_HostedServiceAdded()
        {
            _host.Run();
        }
    }
}
