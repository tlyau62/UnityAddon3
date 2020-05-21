using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.Context;
using UnityAddon.Core.Value;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.Custom
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomAttribute : Attribute
    {
        public string Descriptor { get; set; }
    }

    [Component]
    public class Service
    {
        [Custom(Descriptor = "My_")]
        public string CustomProp { get; set; }
    }

    [Configuration]
    public class CustomConfig : DependencyResolverConfig
    {
        [Bean]
        public override DependencyResolverOption DependencyResolverOption()
        {
            var option = new DependencyResolverOption();

            option.AddResolveStrategy<CustomAttribute>((type, attr, container) =>
                attr.Descriptor + "TestString");

            return option;
        }
    }

    [ComponentScan]
    public class CustomTests : UnityAddonTest
    {
        public CustomTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void Custom()
        {
            Assert.Equal("My_TestString", Service.CustomProp);
        }
    }
}
