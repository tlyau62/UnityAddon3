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

    [ComponentScan(typeof(PrimaryBeanTests))]
    public class PrimaryBeanTests : UnityAddonTest
    {
        [Dependency]
        public IService PrimaryB { get; set; }

        [Fact]
        public void PrimaryBean()
        {
            Assert.IsType<ServiceB>(PrimaryB);
        }
    }
}
