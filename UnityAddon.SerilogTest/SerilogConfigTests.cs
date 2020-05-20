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
using Xunit;

namespace UnityAddon.SerilogTest
{
    public class TestConfig
    {
        [Bean]
        public virtual Action<HostBuilderContext, LoggerConfiguration> LoggerConfig(IConfiguration config)
        {
            return (hostContext, loggerConfig) => loggerConfig.WriteTo.InMemory();
        }
    }

    [ContextConfiguration(typeof(TestConfig), typeof(SerilogConfig))]
    public class SerilogConfigTests : UnityAddonTest
    {
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
