using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core
{
    public class ComponentScanNamespaceExcludeFilter
    {
        public string[] Values { get; set; }

        public ComponentScanNamespaceExcludeFilter(params string[] values)
        {
            Values = values;
        }
    }
}
