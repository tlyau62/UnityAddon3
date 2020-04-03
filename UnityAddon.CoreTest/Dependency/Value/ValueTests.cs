using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Value;
using Xunit;

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
        public void ValueProvider_ResolveValue_ValueInjected()
        {
            Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureAppConfiguration(config =>
               {
                   config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"serviceType", "Write"},
                    });
               })
               .ScanComponentsUA(GetType().Namespace)
               .BuildUA()
               .RunTestUA(this);

            Assert.Equal(ServiceType.Write, Service.Type);
        }
    }
}
