using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.PrimaryBean
{
    public interface IService
    {
    }

    [Component]
    public class ServiceA : IService { }

    [Component]
    [Primary]
    public class ServiceB : IService { }

    [ComponentScan]
    public class PrimaryBeanTests : UnityAddonTest
    {
        public PrimaryBeanTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IService PrimaryB { get; set; }

        [Fact]
        public void PrimaryBean()
        {
            Assert.IsType<ServiceB>(PrimaryB);
        }
    }
}
