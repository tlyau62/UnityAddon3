using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core
{
    [Configuration]
    public class UnityAddonVendor
    {
        [Bean]
        public virtual ProxyGenerator ProxyGenerator()
        {
            return new ProxyGenerator();
        }
    }
}
