using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.BeanMethod
{
    public interface IService { }

    public interface ICommonService { }

    public class ServiceA : IService { }

    public class ServiceB : ICommonService { }

    public class ServiceC : ICommonService { }

    [Configuration]
    public class Config
    {
        [Bean]
        public virtual IService CreateServiceA()
        {
            return new ServiceA();
        }

        [Bean]
        [Qualifier("B")]
        public virtual ICommonService CreateServiceB()
        {
            return new ServiceB();
        }

        [Bean]
        [Qualifier("C")]
        public virtual ICommonService CreateServiceC()
        {
            return new ServiceC();
        }
    }

    [ComponentScan]
    public class BeanMethodTests : UnityAddonTest
    {
        [Dependency]
        public IService ServiceA { get; set; }

        [Dependency("B")]
        public ICommonService ServiceB { get; set; }

        [Dependency("C")]
        public ICommonService ServiceC { get; set; }

        [Fact]
        public void BeanMethod()
        {
            Assert.IsType<ServiceA>(ServiceA);
            Assert.IsType<ServiceB>(ServiceB);
            Assert.IsType<ServiceC>(ServiceC);
        }
    }
}
