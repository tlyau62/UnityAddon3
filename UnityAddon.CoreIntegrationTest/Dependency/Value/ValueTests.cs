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
using UnityAddon.Core.Context;
using Microsoft.Extensions.DependencyInjection;

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

    [ComponentScan(typeof(ValueTests))]
    public class ValueTests : UnityAddonTest
    {
        public ValueTests() : base(true) { }

        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void Value()
        {
            HostBuilder
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"serviceType", "Write"},
                    });
               })
               .Build()
               .Services
               .GetRequiredService<IUnityAddonSP>()
               .BuildUp(this);

            Assert.Equal(ServiceType.Write, Service.Type);
        }
    }
}
