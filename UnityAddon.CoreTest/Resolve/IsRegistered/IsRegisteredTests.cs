using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;
using Unity.Lifetime;

namespace UnityAddon.CoreTest.Resolve.IsRegistered
{
    public interface IA { }

    [Component]
    public class A : IA { }

    public interface IB { }

    [Component]
    [Qualifier("b")]
    public class B : IB { }

    [Trait("Resolve", "IsRegistered")]
    public class IsRegisteredTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void ApplicationContext_CheckOnRegisteredComponent_ReturnsTrue()
        {
            Assert.True(UnityContainer.IsRegisteredUA<IA>());
        }

        [Fact]
        public void ApplicationContext_CheckOnRegisteredQualifiedComponent_ReturnsTrue()
        {
            Assert.True(UnityContainer.IsRegisteredUA<IB>("b"));
            Assert.True(UnityContainer.IsRegisteredUA<IB>());
        }

        [Fact]
        public void ApplicationContext_CheckOnSelfDefinedType_ReturnsTrue()
        {
            UnityContainer.RegisterTypeUA<IsRegisteredTests, IsRegisteredTests>("test", new ContainerControlledLifetimeManager());

            Assert.True(UnityContainer.IsRegisteredUA<IsRegisteredTests>("test"));
            Assert.True(UnityContainer.IsRegisteredUA<IsRegisteredTests>());
        }
    }
}
