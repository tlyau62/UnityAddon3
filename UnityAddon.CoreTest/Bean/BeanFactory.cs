using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.BeanFactory
{
    /* Construct */
    public interface IRepo { }

    public class Repo : IRepo
    {
    }

    public interface IMananger { }

    public interface IService
    {
        IRepo Repo { get; }
        IMananger Mananger { get; }
        string Value { get; }
        string PostStructValue { get; }
    }

    public class Service : IService
    {
        [Dependency]
        public IRepo Repo { get; set; }

        [OptionalDependency]
        public IMananger Mananger { get; set; }

        public string Value { get; set; }

        public string PostStructValue { get; set; }

        public Service([Value("{profiles.active}")]string value)
        {
            Value = value;
        }

        [PostConstruct]
        public void PostStruct()
        {
            PostStructValue = "test";
        }
    }

    /* AsyncPostConstruct */

    [Component]
    [Scope(ScopeType.Transient)]
    public class AsyncRepoA { }

    [Component]
    [Scope(ScopeType.Transient)]
    public class AsyncRepoB { }

    public interface IAsyncService { }

    public class AsyncService : IAsyncService
    {
        [Dependency]
        public IServiceProvider ServiceProvider { get; set; }

        [PostConstruct]
        public void Init()
        {
            Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    ServiceProvider.GetRequiredService<AsyncRepoB>();
                }
            });

            for (var i = 0; i < 100; i++)
            {
                ServiceProvider.GetRequiredService<AsyncRepoA>();
            }
        }
    }

    /* PostConstructNonProxyBean */

    [Component]
    public class StringStore
    {
        public string TestString { get; set; } = "";
    }

    [Component]
    public class ServiceA
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [PostConstruct]
        public void Init()
        {
            StringStore.TestString += "A";
        }
    }

    [Component]
    public class ServiceB
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [Dependency]
        public ServiceA ServiceA { get; set; }

        [PostConstruct]
        public void Init()
        {
            Assert.NotNull(ServiceA);
            StringStore.TestString += "B";
        }
    }

    [Component]
    public class ServiceC
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [Dependency]
        public ServiceB ServiceB { get; set; }

        [PostConstruct]
        public void Init()
        {
            Assert.NotNull(ServiceB);
            StringStore.TestString += "C";
        }
    }

    public class BeanFactory
    {
        [Fact]
        public void Construct()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IRepo, Repo>();
            serCol.AddSingleton<IService, Service>();
            serCol.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"profiles:active", "dev"}
                }).Build());

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.Same(sp.GetRequiredService<IRepo>(), sp.GetRequiredService<IService>().Repo);
            Assert.Null(sp.GetRequiredService<IService>().Mananger);
            Assert.Equal("dev", sp.GetRequiredService<IService>().Value);
            Assert.Equal("test", sp.GetRequiredService<IService>().PostStructValue);
        }


        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        public void AsyncPostConstruct(int loop)
        {
            var serCol = new ServiceCollection();

            serCol.AddTransient<AsyncRepoA>();
            serCol.AddTransient<AsyncRepoB>();
            serCol.AddSingleton<IAsyncService, AsyncService>();

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            for (var i = 0; i < loop; i++)
            {
                sp.GetRequiredService<IAsyncService>();
            }
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(0, 2, 1)]
        [InlineData(1, 0, 2)]
        [InlineData(1, 2, 0)]
        [InlineData(2, 1, 0)]
        [InlineData(2, 0, 1)]
        public void PostConstructNonProxyBean(int orderA, int orderB, int orderC)
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<ServiceA>();
            serCol.AddSingleton<ServiceB>();
            serCol.AddSingleton<ServiceC>();
            serCol.AddSingleton<StringStore>();

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            var stringStore = sp.GetRequiredService<StringStore>();
            var resolveOrder = new Type[3];

            resolveOrder[orderA] = typeof(ServiceA);
            resolveOrder[orderB] = typeof(ServiceB);
            resolveOrder[orderC] = typeof(ServiceC);

            foreach (var t in resolveOrder)
            {
                sp.GetRequiredService(t);
            }

            Assert.Equal("ABC", stringStore.TestString);
        }

    }
}
