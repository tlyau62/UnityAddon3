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
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Add(new ContainerBuilderEntry().ConfigureBeanDefinitions(config =>
            {
                config.AddFromComponentScanner(GetType().Assembly, namespaces.Union(new[] { GetType().Namespace }).ToArray());
            }));

            var sp = containerBuilder.Build();

            sp.BuildUp(this);
        }
    }
}
