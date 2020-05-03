using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Value;
using Xunit;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.CoreTest.Dependency.Value
{
    public enum ServiceType
    {
        Print, Write
    }

    [Component]
    public class Service
    {
        [Value("{serviceType}")]
        public ServiceType Type { get; set; }
    }

    [Trait("Dependency", "Value")]
    public class ValueTests
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void Value()
        {
            var host = Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"serviceType", "Write"},
                    });
               })
               .ConfigureContainer<ContainerBuilder>(builder =>
               {
                   builder.Add(new ContainerBuilderEntry().ConfigureBeanDefinitions(config =>
                   {
                       config.AddFromComponentScanner(GetType().Assembly, GetType().Namespace);
                   }));
               })
               .Build();

            host.Services.BuildUp(this);

            Assert.Equal(ServiceType.Write, Service.Type);
        }
    }
}
