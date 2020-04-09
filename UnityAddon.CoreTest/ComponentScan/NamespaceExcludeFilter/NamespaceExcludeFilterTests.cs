using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.Candidate;
using UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B;
using Xunit;

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.A
{
    [Component]
    public class ServiceA : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B
{
    [Component]
    public class ServiceB : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter
{
    public interface ISerivce { }

    [Trait("ComponentScan", "NamespaceExclude")]
    public class NamespaceFilterTests
    {
        [Dependency]
        public ISerivce Service { get; set; }

        [Fact]
        public void BeanDefinitionRegistry_ComponentScanNamespaceExcludeFilter_TargetNamespaceExluced()
        {
            new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace)
                .ConfigureUA<BeanDefintionCandidateSelectorBuilder>(config =>
                {
                    config.AddExcludeFilter(new NamespaceFilter("UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.A"));
                })
                .BuildUA()
                .BuildTestUA(this);

            Assert.IsType<ServiceB>(Service);
        }
    }
}
