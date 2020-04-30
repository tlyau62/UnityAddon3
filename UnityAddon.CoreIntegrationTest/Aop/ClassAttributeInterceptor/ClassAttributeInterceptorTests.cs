using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Microsoft.Extensions.Hosting;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Aop.ClassAttributeInterceptor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncAttribute : Attribute
    {
    }

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
        void Mul2();
        void Mul5();
    }

    [Inc]
    [Component]
    public class Service : IService
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Mul2()
        {
            Counter.Count *= 2;
        }

        public void Mul5()
        {
            Counter.Count *= 5;
        }
    }

    [Trait("Aop", "ClassAttributeInterceptor")]
    public class ClassAttributeInterceptorTests
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Counter Counter { get; set; }

        public ClassAttributeInterceptorTests()
        {
            var host = new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace)
                .ConfigureUA<AopInterceptorContainerBuilder>(config =>
                {
                    config.AddAopIntercetor<IncInterceptor>();
                })
                .BuildUA()
                .BuildTestUA(this);
        }

        [Fact]
        public void BeanAopStrategy_ClassAttributeInterceptor_IncIntercepted()
        {
            Service.Mul2();
            Service.Mul5();
            Assert.Equal(15, Counter.Count);
        }
    }
}
