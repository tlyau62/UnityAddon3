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
    public class ProfileBeanTests
    {
        [Dependency]
        public IService Service { get; set; }

        public ProfileBeanTests()
        {
        }

        [Theory]
        [InlineData("prod", typeof(ProdService))]
        [InlineData("dev", typeof(DevService))]
        public void BuildStrategy_DependencyOnProfileBean_BeanInjected(string activeProfile, Type resolveType)
        {
            Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"profiles:active", activeProfile},
                    });
               })
               .ScanComponentsUA(GetType().Namespace)
               .BuildUA()
               .BuildTestUA(this);

            Assert.IsType(resolveType, Service);
        }
    }
}
