using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.EnumerableDep
{
    public interface IService<T>
    {
    }

    [Component]
    public class ServiceA : IService<int>
    {
    }

    [Component]
    public class ServiceB : IService<int>
    {
    }

    [Component]
    public class ServiceC : IService<int>
    {
    }

    [Trait("Dependency", "EnumerableDep")]
    public class EnumerableDepTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IEnumerable<IService<int>> Services { get; set; }

        [Dependency("ServiceA")]
        public IService<int> ServiceA { get; set; }

        [Dependency("ServiceB")]
        public IService<int> ServiceB { get; set; }

        [Dependency("ServiceC")]
        public IService<int> ServiceC { get; set; }

        [Fact]
        public void BeanDefinitionRegistry_ResolveByNull_BeanResolved()
        {
            Assert.Contains(ServiceA, Services);
            Assert.Contains(ServiceB, Services);
            Assert.Contains(ServiceC, Services);
            Assert.Equal(3, Services.Count());
        }
    }
}
