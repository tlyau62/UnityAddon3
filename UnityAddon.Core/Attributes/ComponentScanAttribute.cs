using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Define the scope (base namespaces) for component scanner to scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ComponentScanAttribute : ConfigArgAttribute
    {
        public ComponentScanAttribute(params string[] namespaces)
        {
            Args.Add(new object[] { UnityAddonTest.CONFIG_PREFIX + "ComponentScan", typeof(ComponentScanConfig), typeof(Type) });
            Args.Add(new object[] { "Namespaces", namespaces, typeof(string[]) });
        }
    }
}
