using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity;
using Unity.Builder;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Serilog;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.SerilogTest
{
    [Configuration]
    public class TestConfig
    {
        [Bean]
        public virtual LoggerConfiguration LoggerConfig()
        {
            var config = new LoggerConfiguration();

            config.WriteTo.InMemory();

            return config;
        }
    }

    [ContextConfiguration(typeof(TestConfig), typeof(SerilogConfig))]
    public class SerilogConfigTests : UnityAddonTest
    {
        public SerilogConfigTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public void ConfigTest()
        {
            var randomLog = Guid.NewGuid().ToString();

            Log.Information(randomLog);

            InMemorySink.Instance.Should()
                .HaveMessage(randomLog)
                .Appearing()
                .Once();
        }
    }
}
