using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace UnityAddon.CoreTest.Bean.GuidBean
{
    public interface IService { }

    [Component]
    public class GeneralService
    {
        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }
    }

    [Component]
    [Guid("b1368cba-7614-4923-9426-8cd4456da29e")]
    public class PrintService : IService
    {
    }

    [Component]
    [Guid("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
    public class WriteService : IService
    {
    }

    [Trait("Bean", "GuidBean")]
    public class GuidBeanTests : IUnityAddonTest
    {
        [Dependency]
        public GeneralService GeneralService { get; set; }

        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }

        private IUnityContainer _container;

        public GuidBeanTests()
        {
        }

        [Fact]
        public void BuildStrategy_DependencyOnGuidBean_BeanInjected()
        {
            _container = new UnityContainer();

            Host.CreateDefaultBuilder()
                .RegisterUnityAddon(_container)
                .ScanComponentUnityAddon("UnityAddon.CoreTest.Bean.GuidBean")
                .LoadTest(this)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<UnityAddonTestService>();
                })
                .LoadTest(this)
                .InitUnityAddon().Build().Run(); // integrate with the asp core IServiceCollection

            Assert.Same(GeneralService.PrintService, PrintService);
            Assert.Same(GeneralService.WriteService, WriteService);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
