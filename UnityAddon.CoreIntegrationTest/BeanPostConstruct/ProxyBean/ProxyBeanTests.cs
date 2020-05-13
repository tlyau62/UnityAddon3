using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using Xunit;

namespace UnityAddon.CoreTest.BeanPostConstruct.ProxyBean
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncAttribute : Attribute
    {
    }

    [Component]
    [AopAttribute(typeof(IncAttribute))]
    public class IncInterceptor : IInterceptor
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            Counter.Count++;
            invocation.Proceed();
        }
    }

    [Component]
    public class Counter
    {
        public int Count { get; set; } = 0;
    }

    public interface IService
    {
    }

    [Inc]
    [Component]
    public class Service : IService
    {
        [Dependency]
        public Counter Counter { get; set; }

        [PostConstruct]
        public void Init()
        {
            Counter.Count++;
        }
    }

    [ComponentScan]
    public class ProxyBeanTests : UnityAddonTest
    {
        public ProxyBeanTests() : base(true) { }

        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Counter Counter { get; set; }

        [Fact]
        public void ProxyBean()
        {
            HostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureContext<AopInterceptorContainerOption>(option =>
                    {
                        option.AddAopIntercetor<IncInterceptor>();
                    });
                })
                .Build()
                .Services
                .GetRequiredService<IUnityAddonSP>()
                .BuildUp(this);

            Assert.Equal(1, Counter.Count);
        }
    }
}
