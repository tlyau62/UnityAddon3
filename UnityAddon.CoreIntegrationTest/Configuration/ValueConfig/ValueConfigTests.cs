using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Configuration.ValueConfig
{
    [Configuration]
    public class Config
    {
        [Value("{serviceType}")]
        public string ServiceType { get; set; }

        [Bean]
        public virtual IBeanDefinitionCollection BeanDefinitions()
        {
            IBeanDefinitionCollection defs = new BeanDefinitionCollection();

            return defs;
        }
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
    public class ValueConfigTests : UnityAddonTest
    {
        public ValueConfigTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public Config Config { get; set; }


        [Fact]
        public void Test()
        {
            Assert.Equal("Write", Config.ServiceType);
        }
    }
}
