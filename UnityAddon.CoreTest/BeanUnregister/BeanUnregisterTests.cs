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
    public class BeanUnregisterTests
    {
        [Fact]
        public void ContainerRegistry_BeanUnregister_BeanUnregistered()
        {
            var appCtx = new ApplicationContext(new UnityContainer(), GetType().Namespace);
            var service = appCtx.Resolve<Service>();

            Assert.False(service.Disposed);
            appCtx.UnregisterType<Service>();
            Assert.True(service.Disposed);

            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => appCtx.Resolve<Service>());

            Assert.Equal($"Type {typeof(Service)} with name '{nameof(Service)}' is unregistered.", ex.Message);
        }
    }
}
