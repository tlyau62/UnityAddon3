using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.BeanFactory
{
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
    }
}
