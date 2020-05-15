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
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.CoreTest.Dependency.Value
{
    public enum ServiceType
    {
        None, Print, Write
    }

    [Component]
    public class Service
    {
        [Value("{serviceType}")]
        public ServiceType Type { get; set; }
    }

    [Configuration]
    public class ValueConfig
    {
        [Bean]
        public virtual IConfiguration Config()
        {
            var confBuilder = new ConfigurationBuilder();

            confBuilder.AddInMemoryCollection(new Dictionary<string, string> {
                {"serviceType", "Write"}
            });

            return confBuilder.Build();
        }
    }

    [ComponentScan]
    public class ValueTests : UnityAddonTest
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void Value()
        {
            Assert.Equal(ServiceType.Write, Service.Type);
        }
    }
}
