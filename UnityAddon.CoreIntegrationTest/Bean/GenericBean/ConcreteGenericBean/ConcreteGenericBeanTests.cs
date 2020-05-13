using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GenericBean.ConcreteGenericBean
{
    public interface IService<T> { }

    [Component]
    public class Service : IService<int> { }

    [ComponentScan]
    public class ConcreteGenericBeanTests : UnityAddonTest
    {
        [Dependency]
        public IService<int> Service { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void ConcreteGenericBean()
        {
            Assert.IsType<Service>(Service);
            Assert.True(Sp.IsRegistered<IService<int>>());
        }
    }
}