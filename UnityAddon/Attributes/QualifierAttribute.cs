using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QualifierAttribute : Attribute
    {
        public string[] Values { get; set; }

        public QualifierAttribute(params string[] values)
        {
            Values = values;
        }
    }
}
