using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ProfileAttribute : Attribute
    {
        public string[] Values { get; set; }

        public ProfileAttribute(params string[] values)
        {
            Values = values;
        }
    }
}
