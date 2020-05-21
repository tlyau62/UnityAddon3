using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Configuration.InternalBeanMethod
{
    public interface IService { }

    public class Service : IService { }

    [Configuration]
    public class Config
    {
        [Bean]
        internal virtual IService Service()
        {
            return new Service();
        }
    }

    [ComponentScan]
    public class InternalBeanMethodTests : UnityAddonTest
    {
        public InternalBeanMethodTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IService Service { get; set; }

        [Fact]
        public void BeanMethod()
        {
            Assert.NotNull(Service);
        }
    }
}
