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
    public class ResolveByBeanNameTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency("TestString")]
        public string TestString { get; set; }

        [Dependency("ServiceA")]
        public IService ServiceA { get; set; }

        [Dependency("ServiceB")]
        public IService ServiceB { get; set; }

        [Fact]
        public void BeanDefinitionRegistry_ResolveByBeanName_BeanResolved()
        {
            Assert.Equal("test", TestString);
            Assert.IsType<ServiceA>(ServiceA);
            Assert.IsType<ServiceB>(ServiceB);
        }
    }
}
