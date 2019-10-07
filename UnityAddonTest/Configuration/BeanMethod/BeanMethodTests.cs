using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Configuration.BeanMethod
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

    [Trait("Configuration", "BeanMethod")]
    public class BeanMethodTests
    {
        [Fact]
        public void ConfigurationParser_ConfigureBean_BeanRegistered()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var serviceA = container.Resolve<IService>();
            var serviceB = container.Resolve<ICommonService>("B");
            var serviceC = container.Resolve<ICommonService>("C");

            Assert.IsType<ServiceA>(serviceA);
            Assert.IsType<ServiceB>(serviceB);
            Assert.IsType<ServiceC>(serviceC);
        }
    }
}
