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

    [ComponentScan]
    public class GenericDefinitionBeanTests : UnityAddonTest
    {
        [Dependency]
        public IService<int> Service { get; set; }

        [Fact]
        public void GenericDefinitionBean()
        {
            Assert.IsType<Service<int>>(Service);
        }
    }
}
