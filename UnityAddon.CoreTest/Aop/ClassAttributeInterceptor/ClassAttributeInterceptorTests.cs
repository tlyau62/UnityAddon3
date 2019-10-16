using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
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

    [Component]
    public class IncInterceptor : IAttributeInterceptor<IncAttribute>
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
        private ApplicationContext _appContext;
        private IService _service;
        private Counter _counter;

        public ClassAttributeInterceptorTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace);
            _service = _appContext.Resolve<IService>();
            _counter = _appContext.Resolve<Counter>();
        }

        [Fact]
        public void BeanAopStrategy_ClassAttributeInterceptor_IncIntercepted()
        {
            _service.Mul2();
            _service.Mul5();
            Assert.Equal(15, _counter.Count);
        }
    }
}
