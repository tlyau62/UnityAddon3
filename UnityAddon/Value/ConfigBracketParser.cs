using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

        private static readonly Regex DefaultValue = new Regex("^([^:\n]*)(:([^:\n]*))?$");

        public ConfigBracketParser([OptionalDependency]IConfiguration defaultConfig)
        {
            _defaultConfig = defaultConfig;
        }

        protected override string Process(string intermediateResult)
        {
            if (!DefaultValue.IsMatch(intermediateResult))
            {
                throw new FormatException();
            }

            var config = ContainerRegistry?.Resolve<IConfiguration>() ?? _defaultConfig;
            var match = DefaultValue.Matches(intermediateResult);
            var propVal = match[0].Groups[1].Value;
            var hasDefaultValue = match[0].Groups[2].Value != "";
            var defaultVal = match[0].Groups[3].Value;
            var configVal = config[propVal.Replace('.', ':')];

            if (configVal == null && !hasDefaultValue)
            {
                throw new InvalidOperationException($"Fail to find the property '{propVal}'.");
            }

            return configVal ?? defaultVal;
        }
    }
}
