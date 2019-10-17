using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.BeanPostConstruct.ProxyBean
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

    [Trait("BeanPostConstruct", "ProxyBean")]
    public class ProxyBeanTests
    {
        [Fact]
        public void BuildStrategy_PostConstructProxyBean_BeanPostConstructed()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            appContext.Resolve<IService>();

            Assert.Equal(1, appContext.Resolve<Counter>().Count);
        }
    }
}
