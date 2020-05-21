using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GenericBean.GenericDefinitionBean
{
    public interface IService<T> { }

    public interface IComplexService<T, U> { }

    [Component]
    public class Service<T> : IService<T> { }

    [Component]
    public class ComplexService<T, U> : IComplexService<T, U> { }

    [ComponentScan]
    public class GenericDefinitionBeanTests : UnityAddonTest
    {
        public GenericDefinitionBeanTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IService<int> Service { get; set; }

        [Dependency]
        public IComplexService<int, string> ComplexService { get; set; }

        [Fact]
        public void GenericDefinitionBean()
        {
            Assert.IsType<Service<int>>(Service);
            Assert.IsType<ComplexService<int, string>>(ComplexService);
        }
    }
}
