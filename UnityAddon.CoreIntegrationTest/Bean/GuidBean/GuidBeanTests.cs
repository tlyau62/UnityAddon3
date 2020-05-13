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
using UnityAddon.Core.BeanDefinition;
using System.Linq;
using System.Reflection;
using Unity.Lifetime;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Core.Bean;

namespace UnityAddon.CoreIntegrationTest.Bean.GuidBean
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

    [Guid("b1368cba-7614-4923-9426-8cd4456da29e")]
    [Component]
    public class PrintService : IService
    {
    }

    [Guid("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
    [Component]
    public class WriteService : IService
    {
    }

    [ComponentScan]
    public class GuidBeanTests : UnityAddonTest
    {
        [Dependency]
        public GeneralService GeneralService { get; set; }

        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }

        [Fact]
        public void GuidBean()
        {
            Assert.Same(GeneralService.PrintService, PrintService);
            Assert.Same(GeneralService.WriteService, WriteService);
        }
    }
}
