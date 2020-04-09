using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
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

    [Trait("Bean", "PrimaryBean")]
    public class PrimaryBeanTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IService PrimaryB { get; set; }

        [Fact]
        public void BuildStrategy_DependencyOnPrimaryBean_BeanInjected()
        {
            Assert.IsType<ServiceB>(PrimaryB);
        }
    }
}
