using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class ValueAttribute : Attribute
    {
        public string Value { get; set; }

        public ValueAttribute(string value)
        {
            Value = value;
        }
    }
}
