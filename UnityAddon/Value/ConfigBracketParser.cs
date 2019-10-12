using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Attributes;

namespace UnityAddon.Value
{
    [Component]
    public class ConfigBracketParser : AbstrackBracketParser
    {
        public IConfiguration _configuration;

        public ConfigBracketParser([OptionalDependency] IConfiguration configuration)
        {
            _configuration = configuration ?? GetDefaultConfiguration();
        }

        protected virtual IConfiguration GetDefaultConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        protected override string Process(string intermediateResult)
        {
            return _configuration[intermediateResult.Replace('.', ':')];
        }
    }
}
