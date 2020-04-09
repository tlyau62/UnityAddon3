using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GenericBean.GenericDefinitionBean
{
    public interface IService<T> { }

    [Component]
    public class Service<T> : IService<T> { }

    [Trait("Bean", "GenericBean/GenericDefinitionBean")]
    public class GenericDefinitionBeanTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IService<int> Service { get; set; }

        [Fact]
        public void BuildStrategy_DependencyOnGenericDefinitionBean_BeanInjected()
        {
            Assert.IsType<Service<int>>(Service);
        }
    }
}
