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
    public class CachingTests
    {
        [Fact]
        public void ConfigurationParser_ConfigureSingletonBean_BeanCached()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var config = appContext.Resolve<Config>();
            var strA = appContext.Resolve<string>("a");
            var strC = appContext.Resolve<string>("c");

            Assert.Equal("strAstrBstrC", strA);
            Assert.Equal("strC", strC);
            Assert.Equal(2, config.Counter);
        }
    }
}
