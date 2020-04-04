using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Lifetime;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.BeanUnregister
{
    [Component]
    public class Service : IDisposable
    {
        public bool Disposed = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }

    [Trait("BeanUnRegister", "BeanUnRegister")]
    public class BeanUnregisterTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void ContainerRegistry_BeanUnregister_BeanUnregistered()
        {
            var service = UnityContainer.ResolveUA<Service>();

            Assert.True(UnityContainer.IsRegisteredUA<Service>());
            Assert.False(service.Disposed);

            UnityContainer.UnregisterUA<Service>();

            Assert.False(UnityContainer.IsRegisteredUA<Service>());
            Assert.True(service.Disposed);

            Assert.Throws<NoSuchBeanDefinitionException>(() => UnityContainer.ResolveUA<Service>());
        }
    }
}
