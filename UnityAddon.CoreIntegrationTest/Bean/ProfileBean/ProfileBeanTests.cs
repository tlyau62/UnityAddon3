using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
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

    [ComponentScan]
    public class ProfileBeanTests : UnityAddonTest
    {
        public ProfileBeanTests() : base(true)
        {
        }

        [Dependency]
        public IService Service { get; set; }

        [Theory]
        [InlineData("prod", typeof(ProdService))]
        [InlineData("dev", typeof(DevService))]
        public void ProfileBean(string activeProfile, Type resolveType)
        {
            Environment.SetEnvironmentVariable("UNITY_ADDON_PROFILES", activeProfile);

            HostBuilder.Build().Services.GetRequiredService<IUnityAddonSP>().BuildUp(this);

            Assert.IsType(resolveType, Service);
        }
    }
}
