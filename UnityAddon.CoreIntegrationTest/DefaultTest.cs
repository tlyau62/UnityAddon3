using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core
{
    public abstract class DefaultTest
    {
        public DefaultTest(params string[] namespaces)
        {
            var host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new UnityAddonServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.Add(new ContainerBuilderEntry().ConfigureBeanDefinitions(config =>
                    {
                        config.AddFromComponentScanner(GetType().Assembly, namespaces.Union(new[] { GetType().Namespace }).ToArray());
                    }));
                })
                .Build();

            host.Services.BuildUp(this);
        }
    }
}
