using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.EnumBeanDefinition
{
    [Configuration]
    public class EnumBeanDefinitionConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection BeanDefinitions()
        {
            IBeanDefinitionCollection defs = new BeanDefinitionCollection();

            defs.Add(new InstanceBeanDefintion(typeof(string), "test", null, ScopeType.Singleton));

            return defs;
        }
    }

    [ComponentScan]
    public class EnumBeanDefinitionTests : UnityAddonTest
    {
        public EnumBeanDefinitionTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public string TestString { get; set; }

        [Fact]
        public void EnumBeanDefinition()
        {
            Assert.Equal("test", TestString);
        }
    }
}
