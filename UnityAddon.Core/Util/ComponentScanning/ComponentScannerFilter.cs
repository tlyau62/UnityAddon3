using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UnityAddon.Core.Util.ComponentScanning
{
    public static class ComponentScannerFilter
    {
        public static Func<Type, bool> CreateNamepsaceFilter(params string[] namespaces)
        {
            return t => namespaces.Contains(t.Namespace);
        }
    }
}
