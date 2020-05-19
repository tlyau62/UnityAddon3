using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContextConfigurationAttribute : ConfigArgAttribute
    {
        public ContextConfigurationAttribute(params Type[] classes)
        {
            foreach (var config in classes)
            {
                Args.Add(new object[] { UnityAddonTest.CONFIG_PREFIX + config.Name, config, typeof(Type) });
            }
        }
    }
}
