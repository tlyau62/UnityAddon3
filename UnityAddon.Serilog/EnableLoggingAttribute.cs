using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;

namespace UnityAddon.Serilog
{
    public class EnableLoggingAttribute : ConfigArgAttribute
    {
        public EnableLoggingAttribute()
        {
            var config = typeof(SerilogConfig);

            Args.Add(new object[] { UnityAddonTestFixture.CONFIG_PREFIX + config.Name, config, typeof(Type) });
        }
    }
}
