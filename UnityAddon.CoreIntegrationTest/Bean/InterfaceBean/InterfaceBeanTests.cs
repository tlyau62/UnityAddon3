using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using Xunit;

namespace UnityAddon.CoreTest.Bean.InterfaceBean
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class NoImplComponentAttribute : ComponentAttribute
    {
    }

    [NoImplComponent]
    public interface INoImplService
    {
        public void Test();
    }

    [AopAttribute(typeof(NoImplComponentAttribute))]
    public class NoImplInterceptor : IInterceptor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Intercept(IInvocation invocation)
        {
            Logger.Log += "A";
        }
    }

    [Component]
    public class Logger
    {
        public string Log = "";
    }

    [ComponentScan]
    public class InterfaceBeanTests : UnityAddonTest
    {
        [Dependency]
        public INoImplService NoImplService { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        public InterfaceBeanTests() : base(true)
        {
            HostBuilder
                .ConfigureContainer<ApplicationContext>(ctx => ctx.ConfigureContext<AopInterceptorContainerOption>(option => option.AddAopIntercetor<NoImplInterceptor>()))
                .Build()
                .Services
                .GetRequiredService<IUnityAddonSP>()
                .BuildUp(this);
        }

        [Fact]
        public void InterfaceBean()
        {
            NoImplService.Test();

            Assert.Equal("A", Logger.Log);
        }
    }
}
