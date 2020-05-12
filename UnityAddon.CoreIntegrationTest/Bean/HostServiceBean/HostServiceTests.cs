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

namespace UnityAddon.CoreTest.Bean.HostServiceBean
{
    [Component]
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

    [ComponentScan(typeof(HostServiceTests))]
    public class HostServiceTests : UnityAddonTest
    {
        [Dependency]
        public IHost Host { get; set; }

        [Fact]
        public void HostService()
        {
            Host.Run();
        }
    }
}
