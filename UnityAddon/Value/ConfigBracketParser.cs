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
        [OptionalDependency]
        public IContainerRegistry ContainerRegistry { get; set; } // optionalDep for testing

        private IConfiguration _defaultConfig;

        public ConfigBracketParser([OptionalDependency]IConfiguration defaultConfig)
        {
            _defaultConfig = defaultConfig;
        }

        protected override string Process(string intermediateResult)
        {
            var config = ContainerRegistry?.Resolve<IConfiguration>() ?? _defaultConfig;

            return config[intermediateResult.Replace('.', ':')];
        }
    }
}
