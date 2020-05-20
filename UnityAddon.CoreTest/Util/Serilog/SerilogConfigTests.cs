using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Builder;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Util.Serilog;
using Xunit;

namespace UnityAddon.CoreTest.Util.Serilog
{
    public class TestConfig
    {
        [Bean]
        public virtual Action<HostBuilderContext, LoggerConfiguration> LoggerConfig(IConfiguration config)
        {
            return (hostContext, loggerConfig) => loggerConfig.ReadFrom.Configuration(config);
        }

        [Bean]
        public virtual IConfiguration Config()
        {
            var config = new ConfigurationBuilder().AddJsonFile(@"C:\Users\tyautl\Github\UnityAddon3\UnityAddon.CoreTest\Util\Serilog\appsettings.json")
                .Build();

            return config;
        }
    }

    [ContextConfiguration(typeof(TestConfig), typeof(SerilogConfig))]
    public class SerilogConfigTests : UnityAddonTest
    {
        //[Dependency]
        //public ILogger<SerilogConfigTests> Logger { get; set; }

        [Fact]
        public void ConfigTest()
        {
            Log.Debug("You should click the clap button if you found this post useful2!");
        }
    }
}
