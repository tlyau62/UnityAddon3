using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.Candidate;
using UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B;
using Xunit;
using UnityAddon.Core.Util.ComponentScanning;

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
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Add(new ContainerBuilderEntry().ConfigureBeanDefinitions(config =>
            {
                config.AddFromComponentScanner(
                    config => config.IncludeFilters.Add(t => t.Namespace == "UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B"),
                    GetType().Assembly,
                    GetType().Namespace);
            }));

            var sp = containerBuilder.Build();

            sp.BuildUp(this);

            Assert.IsType<ServiceB>(Service);
        }
    }
}
