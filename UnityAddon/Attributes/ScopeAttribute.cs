using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopeAttribute : Attribute
    {
        public Type Value { get; set; }
    }
}
