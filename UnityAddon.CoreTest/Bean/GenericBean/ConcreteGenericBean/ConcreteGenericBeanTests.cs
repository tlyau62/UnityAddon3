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

    [Trait("Bean", "GenericBean/ConcreteGenericBean")]
    public class ConcreteGenericBeanTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IService<int> Service { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        [Fact]
        public void BuildStrategy_DependencyOnConcreteGenericBean_BeanInjected()
        {
            Assert.IsType<Service>(Service);
            Assert.True(Container.IsRegisteredUA<IService<int>>());
        }
    }
}