﻿using Microsoft.Extensions.Configuration;
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
    [Component]
    public class ConfigBracketParser : AbstrackBracketParser
    {
        [OptionalDependency]
        public IContainerRegistry ContainerRegistry { get; set; } // optionalDep for testing

        // [OptionalDependency]
        public IConfiguration Config { get; set; }

        private static readonly Regex DefaultValue = new Regex("^([^:\n]*)(:([^:\n]*))?$", RegexOptions.Compiled);

        // [InjectionConstructor]
        public ConfigBracketParser([OptionalDependency]IConfiguration config)
        {
            Config = config ?? new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        //public ConfigBracketParser(IConfiguration defaultConfig)
        //{
        //    Config = defaultConfig;
        //}


        protected override string Process(string intermediateResult)
        {
            if (!DefaultValue.IsMatch(intermediateResult))
            {
                throw new FormatException();
            }

            var match = DefaultValue.Matches(intermediateResult);
            var propVal = match[0].Groups[1].Value;
            var hasDefaultValue = match[0].Groups[2].Value != "";
            var defaultVal = match[0].Groups[3].Value;
            var configVal = Config[propVal.Replace('.', ':')];

            if (configVal == null && !hasDefaultValue)
            {
                throw new InvalidOperationException($"Fail to find the property '{propVal}'.");
            }

            return configVal ?? defaultVal;
        }
    }
}
