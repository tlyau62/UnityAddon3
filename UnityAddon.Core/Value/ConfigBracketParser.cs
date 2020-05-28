using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Value
{
    /// <summary>
    /// Parse any intermediate result from AbstrackBracketParser
    /// into a valud defined in "IConfiguration".
    /// </summary>
    public class ConfigBracketParser : AbstrackBracketParser
    {
        private static readonly Regex DefaultValue = new Regex("^([^:\n]*)(:([^:\n]*))?$", RegexOptions.Compiled);

        private readonly IConfiguration _config;

        public ConfigBracketParser(): this(null)
        {
        }

        public ConfigBracketParser(IConfiguration config)
        {
            _config = config;
        }

        protected override string Process(string intermediateResult)
        {
            if (_config == null)
            {
                return null;
            }

            if (!DefaultValue.IsMatch(intermediateResult))
            {
                throw new FormatException();
            }

            var match = DefaultValue.Matches(intermediateResult);
            var propVal = match[0].Groups[1].Value;
            var hasDefaultValue = match[0].Groups[2].Value != "";
            var defaultVal = match[0].Groups[3].Value;
            var configVal = _config[propVal.Replace('.', ':')];

            if (configVal == null && !hasDefaultValue)
            {
                throw new InvalidOperationException($"Fail to find the property '{propVal}'.");
            }

            return configVal ?? defaultVal;
        }
    }
}
