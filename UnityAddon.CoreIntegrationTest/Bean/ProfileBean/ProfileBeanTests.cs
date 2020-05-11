using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
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

    public class ProfileBeanTests
    {
        [Dependency]
        public IService Service { get; set; }

        [Theory]
        [InlineData("prod", typeof(ProdService))]
        [InlineData("dev", typeof(DevService))]
        public void ProfileBean(string activeProfile, Type resolveType)
        {
            var host = Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"profiles:active", activeProfile},
                    });
               })
               .ConfigureContainer<ApplicationContext>(ctx =>
               {
                   ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(GetType().Assembly, GetType().Namespace));
               })
               .Build();

            ((IUnityAddonSP)host.Services).BuildUp(this);

            Assert.IsType(resolveType, Service);
        }
    }
}
