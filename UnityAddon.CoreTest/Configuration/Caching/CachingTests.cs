using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.Caching
{
    [Configuration]
    public class Config
    {
        public int Counter { get; set; } = 0;

        [Bean]
        [Qualifier("a")]
        public virtual string StrA()
        {
            Counter++;
            return "strA" + StrB();
        }

        public virtual string StrB()
        {
            return "strB" + StrC();
        }

        [Bean]
        [Qualifier("c")]
        public virtual string StrC()
        {
            Counter++;
            return "strC";
        }
    }

    [Trait("Configuration", "Caching")]
    public class CachingTests: UnityAddonDefaultTest
    {
        [Dependency]
        public Config Config { get; set; }

        [Dependency("a")]
        public string StrA { get; set; }

        [Dependency("c")]
        public string StrC { get; set; }

        [Fact]
        public void ConfigurationParser_ConfigureSingletonBean_BeanCached()
        {
            Assert.Equal("strAstrBstrC", StrA);
            Assert.Equal("strC", StrC);
            Assert.Equal(2, Config.Counter);
        }
    }
}
