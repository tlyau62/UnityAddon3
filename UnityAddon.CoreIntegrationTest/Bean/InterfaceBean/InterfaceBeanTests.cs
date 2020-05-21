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
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
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

    [Configuration]
    public class InterfaceBeanConfig : AopInterceptorConfig
    {
        [Bean]
        public override AopInterceptorOption AopInterceptorOption()
        {
            var aopInterceptorOption = new AopInterceptorOption();

            aopInterceptorOption.AddAopIntercetor<NoImplInterceptor>();

            return aopInterceptorOption;
        }
    }

    [ComponentScan]
    public class InterfaceBeanTests : UnityAddonTest
    {
        public InterfaceBeanTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public INoImplService NoImplService { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        [Fact]
        public void InterfaceBean()
        {
            NoImplService.Test();

            Assert.Equal("A", Logger.Log);
        }
    }
}
