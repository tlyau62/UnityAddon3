﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.GeneralBean;
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

            defs.Add(new InstanceBeanDefintion(typeof(string), "test", null, ScopeType.Singleton));

            return defs;
        }
    }

    public class EnumBeanDefinitionTests : DefaultTest
    {
        [Dependency]
        public string TestString { get; set; }

        [Fact]
        public void EnumBeanDefinition()
        {
            Assert.Equal("test", TestString);
        }
    }
}
