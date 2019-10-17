using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Resolve.ResolveByBeanName
{
    [Configuration]
    public class Config
    {
        [Bean]
        public virtual string TestString()
        {
            return "test";
        }
    }

    public interface IService { }

    [Component]
    public class ServiceA : IService
    {
    }

    [Component]
    public class ServiceB : IService
    {
    }

    [Trait("Resolve", "ResolveByBeanName")]
    public class ResolveByBeanNameTests
    {
        [Fact]
        public void BeanDefinitionRegistry_ResolveByBeanName_BeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Equal("test", appContext.Resolve<string>("TestString"));
            Assert.IsType<ServiceA>(appContext.Resolve<IService>("ServiceA"));
            Assert.IsType<ServiceB>(appContext.Resolve<IService>("ServiceB"));
        }
    }
}
