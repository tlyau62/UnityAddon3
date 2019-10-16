using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

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
    public class IsRegisteredTests
    {
        [Fact]
        public void ApplicationContext_CheckOnRegisteredComponent_ReturnsTrue()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.True(appContext.IsRegistered<IA>());
        }

        [Fact]
        public void ApplicationContext_CheckOnRegisteredQualifiedComponent_ReturnsTrue()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.True(appContext.IsRegistered<IB>("b"));
            Assert.True(appContext.IsRegistered<IB>());
        }

        [Fact]
        public void ApplicationContext_CheckOnSelfDefinedType_ReturnsTrue()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            container.RegisterType<IsRegisteredTests>("test");

            Assert.True(appContext.IsRegistered<IsRegisteredTests>("test"));
            Assert.True(appContext.IsRegistered<IsRegisteredTests>());
        }
    }
}
