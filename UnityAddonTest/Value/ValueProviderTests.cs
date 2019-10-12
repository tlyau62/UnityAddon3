using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Value;
using Xunit;

namespace UnityAddonTest.Value
{
    public enum EnumValue
    {
        A, B
    }

    [Trait("Value", "ValueProvider")]
    public class ValueProviderTests
    {
        [Theory]
        [InlineData(typeof(string), "abc", "abc", "abc")]
        [InlineData(typeof(int), "123", "123", 123)]
        [InlineData(typeof(EnumValue), "A", "A", EnumValue.A)]
        public void ValueProvider_ParseValueByType_ValueParsed(Type valType, string valExpr, string valParsed, object expected)
        {
            var provider = new ValueProvider();
            var configParserMock = new Mock<ConfigBracketParser>(null);

            configParserMock.Setup(m => m.Parse(It.Is<string>(str => str == valExpr))).Returns(valParsed);

            provider.ConfigBracketParser = configParserMock.Object;

            Assert.Equal(expected, provider.GetValue(valType, valExpr));
        }
    }
}
