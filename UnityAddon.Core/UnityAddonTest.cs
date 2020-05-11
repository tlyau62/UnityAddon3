using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public static class UnityAddonTestFuncCollection
    {
        public static Action<IHostBuilder, UnityAddonTest> CreateComponentScan(params string[] namespaces)
        {
            return (builder, test) =>
            {
                builder.ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(test.GetType().Assembly, namespaces.Length == 0 ? new[] { test.GetType().Namespace } : namespaces).ToArray());
                });
            };
        }
    }
}

public abstract class UnityAddonTest
{
    public UnityAddonTest(params Action<IHostBuilder, UnityAddonTest>[] hostBuilderConfigs)
    {
        var hostBuilder = Host.CreateDefaultBuilder().RegisterUA();
        hostBuilderConfigs.Aggregate((acc, config) => acc + config)(hostBuilder, this);
        var host = hostBuilder.Build();

        ((IUnityAddonSP)host.Services).BuildUp(GetType(), this);
    }
}

public abstract class UnityAddonComponentScanTest : UnityAddonTest
{
    public UnityAddonComponentScanTest(params string[] namespaces) : base(UnityAddonTestFuncCollection.CreateComponentScan(namespaces))
    {
    }

    public UnityAddonComponentScanTest(Action<IHostBuilder, UnityAddonTest>[] hostBuilderConfigs, params string[] namespaces) : base(hostBuilderConfigs.Union(new[] { UnityAddonTestFuncCollection.CreateComponentScan(namespaces) }).ToArray())
    {
    }
}