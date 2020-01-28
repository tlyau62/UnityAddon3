using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Define the scope (base namespaces) for component scanner to scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method)]
    public class ComponentScanAttribute : Attribute
    {
        public string[] BaseNamespaces { get; set; }
    }
}
