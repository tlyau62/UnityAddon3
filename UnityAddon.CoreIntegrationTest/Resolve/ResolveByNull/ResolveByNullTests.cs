using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Resolve.ResolveByNull
{
    public interface IService { }

    [Component]
    [Qualifier("test")]
    public class Service : IService { }

    [Trait("Resolve", "ResolveByNull")]
    public class ResolveByNullTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency]
        public IService Service { get; set; }

        [Fact]
        public void BeanDefinitionRegistry_ResolveByNull_BeanResolved()
        {
            Assert.IsType<Service>(UnityContainer.ResolveUA<IService>());
            Assert.IsType<Service>(Service);
        }
    }
}
