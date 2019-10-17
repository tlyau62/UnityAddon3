using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class QualifierAttribute : Attribute
    {
        public string[] Values { get; set; } = new string[0];

        public QualifierAttribute(params object[] values)
        {
            Values = values.Select(v => v.ToString()).ToArray();
        }
    }
}
