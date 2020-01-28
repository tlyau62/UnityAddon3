using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// PostConstructor will be executed after all dependencies of a bean are injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstructAttribute: Attribute
    {
    }
}
