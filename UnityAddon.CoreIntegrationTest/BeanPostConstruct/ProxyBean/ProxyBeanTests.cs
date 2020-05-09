using Castle.DynamicProxy;
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

    public class ProxyBeanTests
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Counter Counter { get; set; }

        [Fact]
        public void ProxyBean()
        {
            var host = Host.CreateDefaultBuilder()
                .RegisterUA()
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.AddContextEntry(entry => entry.ConfigureBeanDefinitions(defs => defs.AddFromComponentScanner(GetType().Assembly, GetType().Namespace)));
                    ctx.ConfigureContext<AopInterceptorContainerOption>(option =>
                    {
                        option.AddAopIntercetor<IncInterceptor>();
                    });
                })
                .Build();

            ((IUnityAddonSP)host.Services).BuildUp(this);

            Assert.Equal(1, Counter.Count);
        }
    }
}
