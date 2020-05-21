using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Test.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContextConfigurationAttribute : ConfigArgAttribute
    {
        public ContextConfigurationAttribute(params Type[] classes)
        {
            foreach (var config in classes)
            {
                Args.Add(new object[] { UnityAddonTestFixture.CONFIG_PREFIX + config.Name, config, typeof(Type) });
            }
        }
    }
}
