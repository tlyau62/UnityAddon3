using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportAttribute : Attribute
    {
        public Type[] Configs { get; set; }

        public ImportAttribute(params Type[] configs)
        {
            Configs = configs;
        }
    }
}
