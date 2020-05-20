using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Serilog
{
    public class SerilogConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection Serilog(HostBuilderContext hostBuilderContext, [OptionalDependency] Action<HostBuilderContext, LoggerConfiguration> configureLogger)
        {
            IBeanDefinitionCollection defCol = new BeanDefinitionCollection();

            defCol.AddFromServiceCollection(services =>
            {
                new SerilogServiceCollectionTrap(hostBuilderContext, services).UseSerilog((hostContext, loggerConfig) =>
                {
                    configureLogger(hostContext, loggerConfig);
                });
            });

            return defCol;
        }
    }
}
