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
    public class GuidBeanTests
    {
        [Dependency]
        public GeneralService GeneralService { get; set; }

        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }

        [Fact]
        public void BuildStrategy_DependencyOnGuidBean_BeanInjected()
        {
            Host.CreateDefaultBuilder()
                .RegisterUA()
                .ScanComponentsUA("UnityAddon.CoreTest.Bean.GuidBean")
                .BuildUA()
                .BuildTestUA(this);

            Assert.Same(GeneralService.PrintService, PrintService);
            Assert.Same(GeneralService.WriteService, WriteService);
        }
    }
}
