using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean
{
    [Import(typeof(ConfigA), typeof(ConfigB))]
    public class ConfigMain
    {
    }

    public class ConfigA
    {
        [Bean]
        public virtual string TestA() => "TestStringA";
    }

    [Import(typeof(ConfigC))]
    public class ConfigB
    {
        [Bean]
        public virtual string TestB() => "TestStringB";
    }

    public class ConfigC
    {
        [Bean]
        public virtual string TestC() => "TestStringC";
    }

    [ContextConfiguration(typeof(ConfigMain))]
    public class ImportRegistryTests : UnityAddonTest
    {
        public ImportRegistryTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void Refresh_Import()
        {
            Assert.True(Sp.IsRegistered<ConfigA>());
            Assert.Equal("TestStringA", Sp.GetRequiredService<string>("TestA"));

            Assert.True(Sp.IsRegistered<ConfigB>());
            Assert.Equal("TestStringB", Sp.GetRequiredService<string>("TestB"));

            Assert.True(Sp.IsRegistered<ConfigC>());
            Assert.Equal("TestStringC", Sp.GetRequiredService<string>("TestC"));
        }
    }
}
