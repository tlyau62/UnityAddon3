using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class UnityAddonTest
    {
        public UnityAddonTest(Action<IHostBuilder, UnityAddonTest> hostBuilderConfig)
        {
            var hostBuilder = Host.CreateDefaultBuilder().RegisterUA();
            hostBuilderConfig(hostBuilder, this);
            var host = hostBuilder.Build();

            host.Services.BuildUp(GetType(), this);
        }
    }

    public abstract class UnityAddonComponentScanTest : UnityAddonTest
    {
        public UnityAddonComponentScanTest(params string[] namespaces) : base(Setup(namespaces))
        {
        }

        public UnityAddonComponentScanTest(Action<IHostBuilder, UnityAddonTest> hostBuilderConfig, string[] namespaces) : base(Setup(hostBuilderConfig, namespaces))
        {
        }

        private static Action<IHostBuilder, UnityAddonTest> Setup(string[] namespaces)
        {
            return Setup((builder, test) => { }, namespaces);
        }

        private static Action<IHostBuilder, UnityAddonTest> Setup(Action<IHostBuilder, UnityAddonTest> hostBuilderConfig, string[] namespaces)
        {
            hostBuilderConfig += (builder, test) =>
            {
                builder.ConfigureContainer<ApplicationContext>(builder =>
                 {
                     builder.AddContextEntry(entry =>
                     {
                         entry.ConfigureBeanDefinitions(defs =>
                         {
                             defs.AddFromComponentScanner(test.GetType().Assembly, namespaces.Union(new[] { test.GetType().Namespace }).ToArray());
                         });
                     });
                 });
            };

            return hostBuilderConfig;
        }
    }
}
