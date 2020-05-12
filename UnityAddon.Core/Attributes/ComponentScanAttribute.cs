using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Configs;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Define the scope (base namespaces) for component scanner to scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ComponentScanAttribute : ConfigArgAttribute
    {
        public ComponentScanAttribute(Type testcase, params string[] namespaces)
        {
            Args.Add(new object[] { UnityAddonTest.CONFIG_PREFIX + "ComponentScan", typeof(ComponentScanTestConfig), typeof(Type) });
            Args.Add(new object[] { UnityAddonTest.CONFIG_ARGS_PREFIX + "TestCase", testcase, typeof(Type) });
            Args.Add(new object[] { UnityAddonTest.CONFIG_ARGS_PREFIX + "Namespaces", namespaces, typeof(string[]) });
        }
    }
}
