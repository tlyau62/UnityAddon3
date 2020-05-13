using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.BeanRegistry.PostRegister
{
    public interface IService
    {
        bool Disposed { get; set; }
    }

    public class Service : IService, IDisposable
    {
        public bool Disposed { get; set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }

    [ComponentScan]
    public class PostRegisterTests : UnityAddonTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IServicePostRegistry ServicePostRegistry { get; set; }

        [Fact]
        public void ImplTypeSingleton()
        {
            ServicePostRegistry.AddSingleton(typeof(IService), typeof(Service), null);

            var a = Sp.GetRequiredService<IService>();
            var b = Sp.GetRequiredService<IService>();

            Assert.IsType<Service>(a);
            Assert.Same(a, b);
        }

        [Fact]
        public void ImplTypeTransient()
        {
            ServicePostRegistry.AddTransient(typeof(IService), typeof(Service), null);

            var a = Sp.GetRequiredService<IService>();
            var b = Sp.GetRequiredService<IService>();

            Assert.IsType<Service>(a);
            Assert.IsType<Service>(b);
            Assert.NotSame(a, b);
        }

        [Fact]
        public void Instance()
        {
            var service = new Service();

            ServicePostRegistry.AddSingleton(typeof(IService), service, null);

            Assert.Same(service, Sp.GetRequiredService<IService>());
        }

        [Fact]
        public void FactorySingleton()
        {
            ServicePostRegistry.AddSingleton(typeof(IService), (sp, t, n) => new Service(), null);

            var a = Sp.GetRequiredService<IService>();
            var b = Sp.GetRequiredService<IService>();

            Assert.IsType<Service>(a);
            Assert.Same(a, b);
        }

        [Fact]
        public void FactoryTransient()
        {
            ServicePostRegistry.AddTransient(typeof(IService), (sp, t, n) => new Service(), null);

            var a = Sp.GetRequiredService<IService>();
            var b = Sp.GetRequiredService<IService>();

            Assert.IsType<Service>(a);
            Assert.IsType<Service>(b);
            Assert.NotSame(a, b);
        }

        [Fact]
        public void Qualifier()
        {
            ServicePostRegistry.AddSingleton(typeof(IService), typeof(Service), "testqua");

            Assert.True(Sp.IsRegistered<IService>("testqua"));
            Assert.False(Sp.IsRegistered<IService>("notexist"));
        }

        [Fact]
        public void Unregister()
        {
            ServicePostRegistry.AddSingleton(typeof(IService), typeof(Service), null);

            var service = Sp.GetRequiredService<IService>();

            Assert.True(Sp.IsRegistered<IService>());
            Assert.False(service.Disposed);

            ServicePostRegistry.Unregister(typeof(IService), null);

            Assert.False(Sp.IsRegistered<IService>());
            Assert.True(service.Disposed);
        }
    }
}
