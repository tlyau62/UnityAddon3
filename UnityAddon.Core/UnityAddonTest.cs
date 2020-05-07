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
        public UnityAddonTest(params string[] namespaces)
        {
            var host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new ServiceProviderFactory())
                .ConfigureContainer<ApplicationContext>(builder =>
                {
                    builder.AddContextEntry(entry =>
                    {
                        entry.ConfigureBeanDefinitions(defs => defs.AddFromComponentScanner(GetType().Assembly, namespaces.Union(new[] { GetType().Namespace }).ToArray()));
                    });
                })
                .Build();

            host.Services.BuildUp(GetType(), this);
        }
    }
}
