﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.Candidate;
using Xunit;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.B;
using UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.A;
using UnityAddon.Core.Context;
using Microsoft.Extensions.DependencyInjection;
using UnityAddon.Test;

namespace UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.A
{
    [Component]
    public class ServiceA : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.B
{
    [Component]
    public class ServiceB : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter
{
    public interface ISerivce { }

    public class NamespaceFilterTests : UnityAddonTest
    {
        public NamespaceFilterTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public ISerivce Service { get; set; }

        [Fact]
        public void NamespaceFilterOnB()
        {
            HostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) =>
                    {
                        config.AddFromComponentScanner(
                                config => config.IncludeFilters.Add(ComponentScannerFilter.CreateNamepsaceFilter("UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.B")),
                                GetType().Assembly,
                                GetType().Namespace);
                    });
                })
                .Build()
                .Services
                .GetRequiredService<IUnityAddonSP>()
                .BuildUp(this);

            Assert.IsType<ServiceB>(Service);
        }

        [Fact]
        public void NamespaceFilterOnA()
        {
            HostBuilder
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) =>
                    {
                        config.AddFromComponentScanner(
                                config => config.IncludeFilters.Add(ComponentScannerFilter.CreateNamepsaceFilter("UnityAddon.CoreTest.BeanRegistry.ComponentScan.NamespaceExcludeFilter.A")),
                                GetType().Assembly,
                                GetType().Namespace);
                    });
                })
                .Build()
                .Services
                .GetRequiredService<IUnityAddonSP>()
                .BuildUp(this);

            Assert.IsType<ServiceA>(Service);
        }
    }
}
