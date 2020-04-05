using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.DependencyInjection;
using UnityAddon.Core.Value;
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

    [Trait("Dependency", "Custom")]
    public class CustomTests
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void DependencyResolverBuilder_ResolveCustomValue_CustomValueInjected()
        {
            Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureUA<DependencyResolverBuilder>(config =>
               {
                   config.AddResolveStrategy<CustomAttribute>((type, attr, container)
                       => attr.Descriptor + "TestString");
               })
               .ScanComponentsUA(GetType().Namespace)
               .BuildUA()
               .BuildTestUA(this);

            Assert.Equal("My_TestString", Service.CustomProp);
        }
    }
}
