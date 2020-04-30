using Castle.DynamicProxy;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
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

    [Trait("Bean", "GuidBean")]
    public class InterfaceBeanTests
    {
        [Dependency]
        public INoImplService NoImplService { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        public InterfaceBeanTests()
        {
            Host.CreateDefaultBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace)
                .ConfigureUA<AopInterceptorContainerBuilder>(c =>
                {
                    c.AddAopIntercetor<NoImplInterceptor>();
                })
                .BuildUA()
                .BuildTestUA(this);
        }

        [Fact]
        public void BeanFactory_CreateInterfaceBean_InterfaceBeanCreated()
        {
            NoImplService.Test();

            Assert.Equal("A", Logger.Log);
        }
    }
}
