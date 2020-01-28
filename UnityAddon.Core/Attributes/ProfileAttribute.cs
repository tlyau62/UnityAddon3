using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Profiled bean will be created only when its profile is included in the active profiles.
    /// </summary>
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
