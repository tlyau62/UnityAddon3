using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using Xunit;
using Unity.Lifetime;

namespace UnityAddon.CoreTest.Resolve.ResolveGeneric
{
    public interface IService<T> { }

    public class Service<T> : IService<T>
    {
    }

    [Trait("Resolve", "ResolveGeneric")]
    public class ResolveGenericTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void BeanDefinitionRegistry_ResolveGeneric_BeanResolved()
        {
            UnityContainer.RegisterTypeUA(null, typeof(IService<>), typeof(Service<>), new ContainerControlledLifetimeManager());

            Assert.NotNull(UnityContainer.ResolveUA<IService<int>>());
        }
    }
}
