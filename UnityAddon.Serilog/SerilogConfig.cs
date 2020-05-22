using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Serilog
{
    [Configuration]
    public class SerilogConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection Serilog(HostBuilderContext hostBuilderContext, [OptionalDependency] LoggerConfiguration loggerConfig)
        {
            IBeanDefinitionCollection defCol = new BeanDefinitionCollection();

            loggerConfig ??= new LoggerConfiguration();

            Log.Logger = loggerConfig.CreateLogger();

            defCol.AddFromServiceCollection(services => services.AddLogging(logging => logging.AddSerilog(Log.Logger)));

            return defCol;
        }
    }
}
