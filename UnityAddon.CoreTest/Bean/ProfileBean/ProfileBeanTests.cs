using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.Bean.ProfileBean
{
    public interface IService
    {
    }

    [Component]
    [Profile("dev")]
    public class DevService : IService
    {
    }

    [Component]
    [Profile("prod")]
    public class ProdService : IService
    {
    }

    [Trait("Bean", "ProfileBean")]
    public class ProfileBeanTests : UnityAddonTest
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        public ProfileBeanTests()
        {
        }

        [Theory]
        [InlineData("prod", typeof(ProdService))]
        //[InlineData("dev", typeof(DevService))]
        public void BuildStrategy_DependencyOnProfileBean_BeanInjected(string activeProfile, Type resolveType)
        {
            Host.CreateDefaultBuilder()
                .RegisterUA(config =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"profiles:active", activeProfile},
                    });
                })
                .ScanComponentUA(GetType().Namespace)
                .InitUA()
                .EnableTestMode(this)
                .Build();

            Assert.IsType(resolveType, Container.ResolveUA<IService>());
        }

        //[Theory]
        //[InlineData("prod", typeof(DevService))]
        //[InlineData("dev", typeof(ProdService))]
        //public void BuildStrategy_DependencyOnNonActiveProfileBean_ExceptionThrown(string activeProfile, Type resolveType)
        //{
        //    IHost host = Host.CreateDefaultBuilder()
        //        .RegisterUnityAddon()
        //        .ScanComponentUnityAddon(GetType().Namespace)
        //        .InitUnityAddon()
        //        .EnableTestMode(this)
        //        .ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            config.AddInMemoryCollection(new Dictionary<string, string>
        //            {
        //                {"profiles:active", activeProfile},
        //            });
        //        }).Build();

        //    Assert.Throws<Exception>(() => Container.ResolveUA(resolveType));
        //}
    }
}
