using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using Xunit;

namespace UnityAddon.CoreTest
{
    public interface IA
    {
        bool Disposed { get; }
    }

    public class A : IA, IDisposable
    {
        public bool Disposed { get; set; }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    public class A2 : IA
    {
        public bool Disposed => true;
    }

    public class ServiceProvider
    {
        [Fact]
        public void GetService()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IA, A>();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.IsType<A>(usp.GetService<IA>());
        }

        [Fact]
        public void GetAllServices()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IA, A>();
            serCol.AddSingleton<IA, A2>();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();
            var serAs = new IA[] { usp.GetService<IA>("A"), usp.GetService<IA>("A2") };

            Assert.Equal(usp.GetServices<IA>(), serAs);
            Assert.IsType<IA[]>(usp.GetServices<IA>());
        }

        [Fact]
        public void GetRequiredService()
        {
            var serCol = new ServiceCollection();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.Throws<InvalidOperationException>(() => usp.GetRequiredService<IA>());
        }

        [Fact]
        public void CreateScope()
        {
            var serCol = new ServiceCollection();

            serCol.AddScoped<IA, A>();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            IA a, b, c;

            using (var scope1 = usp.CreateScope())
            {
                a = scope1.ServiceProvider.GetService<IA>();
                b = scope1.ServiceProvider.GetService<IA>();

                Assert.False(a.Disposed);
                Assert.False(b.Disposed);

                using (var scope2 = usp.CreateScope())
                {
                    c = scope2.ServiceProvider.GetService<IA>();
                    Assert.False(c.Disposed);

                    Assert.Same(a, b);
                    Assert.NotSame(a, c);
                }

                Assert.True(c.Disposed);
            }

            Assert.True(a.Disposed);
            Assert.True(b.Disposed);
        }

        [Fact]
        public void IsRegistered()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IA, A>();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.True(usp.IsRegistered(typeof(IA)));
            Assert.False(usp.IsRegistered(typeof(A)));
        }

        [Fact]
        public void CanResolve()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IA, A>();

            var usp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.True(usp.CanResolve(typeof(IA)));
            Assert.True(usp.CanResolve(typeof(IEnumerable<IA>)));
            Assert.False(usp.CanResolve(typeof(A)));
        }
    }
}
