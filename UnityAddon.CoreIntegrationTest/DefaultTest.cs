using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Bootstrap;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class DefaultTest
    {
        public DefaultTest(params string[] namespaces)
        {
            var host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new ServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.AddContextEntry(entry =>
                    {
                        entry.ConfigureBeanDefinitions(defs => defs.AddFromComponentScanner(GetType().Assembly, namespaces.Union(new[] { GetType().Namespace }).ToArray()));
                    });
                })
                .Build();

            host.Services.BuildUp(this);
        }
    }
}
