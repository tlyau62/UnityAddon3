using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Indicate a bean method used to declare a bean.
    /// Must be declared inside a configuration class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeanAttribute: Attribute
    {
    }
}
