using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Value;
using Xunit;

namespace UnityAddon.CoreTest.Value
{
    [Trait("Value", "ConfigBracketParser")]
    public class ConfigBracketParserTests
    {
        private IConfiguration _config;
        private static Dictionary<string, string> arrayDict = new Dictionary<string, string>
        {
            {"str1", "value1"},
            {"str2", "value2"},
            {"str3", "value3"},
            {"index", "3"},
            { "testvalue3value3", "133" },
            { "test:test2:test3", "123" }
        };

        public ConfigBracketParserTests()
        {
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(arrayDict)
                .Build();
        }

        [Theory]
        [InlineData("teststring", "teststring")]
        [InlineData("test{str1}{str2}", "testvalue1value2")]
        [InlineData("test{str{index}}{str{index}}", "testvalue3value3")]
        [InlineData("{test{str{index}}{str{index}}}", "133")]
        [InlineData("test{str1}{str2}test", "testvalue1value2test")]
        [InlineData("test{test.test2.test3}test", "test123test")]
        public void ConfigBracketParser_ValueExpression_Parsed(string input, string expected)
        {
            var parser = new ConfigBracketParser();

            parser.Config = _config;

            Assert.Equal(expected, parser.Parse(input));
        }

        [Theory]
        [InlineData("test{undefined:qqq}test", "testqqqtest")]
        [InlineData("test{undefined:}test", "testtest")]
        public void ConfigBracketParser_DefaultValueExpression_Parsed(string input, string expected)
        {
            var parser = new ConfigBracketParser();

            parser.Config = _config;

            Assert.Equal(expected, parser.Parse(input));
        }
    }
}
