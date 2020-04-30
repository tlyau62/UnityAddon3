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
        public PrintService()
        {
            var a = 10;
        }
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
            var f = new ServiceProviderFactory();
            var c = f.CreateBuilder(new ServiceCollection());
            var a = f.CreateServiceProvider(c);

            var s = a.GetRequiredService<ComponentScanner>();

            var dfs = s.ScanComponents(Assembly.GetExecutingAssembly(), "UnityAddon.CoreTest.Bean.GuidBean");

            var dc = a.GetRequiredService<IBeanDefinitionContainer>();

            dc.RegisterBeanDefinitions(dfs);

            foreach (var df in dfs)
            {
                c.RegisterFactory(df.Type, df.Name, (c, t, n) => df.Constructor(a, t, n), (IFactoryLifetimeManager)df.Scope);
            }

            var r = dfs.Last();
            var y = c.Resolve(r.Type, r.Name) == c.Resolve(r.Type, r.Name);

            var o = c.Resolve(r.Type, r.Qualifiers.First());

            var u = c.Resolve(r.Type, r.Qualifiers.First()) == c.Resolve(r.Type, r.Qualifiers.First());



            //IUnityContainer container = new UnityContainer();

            //var host = Host.CreateDefaultBuilder()
            //    .RegisterUA(container)
            //    .ScanComponentsUA("UnityAddon.CoreTest.Bean.GuidBean")
            //    .Build();

            //var t = container.Resolve<IBeanDefinitionContainer>().GetAllBeanDefinitions(typeof(IService)).Last();

            //var l = container.Resolve(t.AutoWiredTypes[1], t.Name) == container.Resolve(t.AutoWiredTypes[1], t.Name);

            var x = c.Resolve<IService>("4e55e61a-c57f-4b55-84dd-044d539dfbc7");
            var z = c.Resolve<IService>("4e55e61a-c57f-4b55-84dd-044d539dfbc7");
            var q = x == z;

            a.BuildUp(this);

            Assert.Same(GeneralService.PrintService, PrintService);
            Assert.Same(GeneralService.WriteService, WriteService);
        }
    }
}
