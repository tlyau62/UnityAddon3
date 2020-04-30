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
using UnityAddon.Core.Component;
using System.Reflection;
using Unity.Lifetime;

namespace UnityAddon.CoreTest.Bean.GuidBean
{
    public interface IService { }

    public class GeneralService
    {
        [Dependency("b1368cba-7614-4923-9426-8cd4456da29e")]
        public IService PrintService { get; set; }

        [Dependency("4e55e61a-c57f-4b55-84dd-044d539dfbc7")]
        public IService WriteService { get; set; }
    }

    [Guid("b1368cba-7614-4923-9426-8cd4456da29e")]
    public class PrintService : IService
    {
        public PrintService()
        {
            var a = 10;
        }
    }

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
            var f = new ServiceProviderFactory();
            var defCol = f.CreateBuilder(new ServiceCollection());

            defCol.AddComponent<GeneralService>()
                .AddComponent<PrintService>()
                .AddComponent<WriteService>();

            var a = f.CreateServiceProvider(defCol);

            a.BuildUp(this);

            Assert.Same(GeneralService.PrintService, PrintService);
            Assert.Same(GeneralService.WriteService, WriteService);
        }
    }
}
