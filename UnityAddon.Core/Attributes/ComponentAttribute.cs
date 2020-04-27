using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Marked class will be included during component scanning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ComponentAttribute : Attribute
    {
    }
}
