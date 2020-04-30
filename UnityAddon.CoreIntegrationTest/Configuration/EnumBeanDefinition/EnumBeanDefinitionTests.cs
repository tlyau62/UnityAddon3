using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.EnumBeanDefinition
{
    [Configuration]
    public class EnumBeanDefinitionConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection BeanDefinitions()
        {
            var defs = new BeanDefinitionCollection();

            defs.Add(new SimpleFactoryBeanDefinition(typeof(string), (c, t, s) => "test"));

            return defs;
        }
    }

    public class EnumBeanDefinitionTests : DefaultTest
    {
        [Dependency]
        public string TestString { get; set; }

        [Fact]
        public void SimpleFactoryBeanDefinition_EnumBeanDefinition_BeanDefitionRegistered()
        {
            Assert.Equal("test", TestString);
        }
    }
}
