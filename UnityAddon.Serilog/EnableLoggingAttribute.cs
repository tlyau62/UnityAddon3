using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.CoreTest.Bean;

namespace UnityAddon.Serilog
{
    public class EnableLoggingAttribute : ConfigArgAttribute
    {
        public EnableLoggingAttribute()
        {
            var config = typeof(SerilogConfig);

            Args.Add(new object[] { UnityAddonTest.CONFIG_PREFIX + config.Name, config, typeof(Type) });
        }
    }
}
