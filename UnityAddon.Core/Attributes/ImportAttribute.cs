using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ImportAttribute : ConfigArgAttribute
    {
        public ImportAttribute(params Type[] configs)
        {
            foreach (var config in configs)
            {
                Args.Add(new object[] { UnityAddonTest.CONFIG_PREFIX + config.Name, config, typeof(Type) });
            }
        }
    }
}
