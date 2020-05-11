using Microsoft.Extensions.Configuration;
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

    public class CustomTests
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void Custom()
        {
            var host = Host.CreateDefaultBuilder()
               .RegisterUA()
               .ConfigureContainer<ApplicationContext>(ctx =>
               {
                   ctx.ConfigureBeans((config, sp) => config.AddComponent(typeof(Service)));

                   ctx.ConfigureContext<DependencyResolverOption>(config =>
                   {
                       config.AddResolveStrategy<CustomAttribute>((type, attr, container)
                           => attr.Descriptor + "TestString");
                   });
               })
               .Build();

            ((IUnityAddonSP)host.Services).BuildUp(this);

            Assert.Equal("My_TestString", Service.CustomProp);
        }
    }
}
