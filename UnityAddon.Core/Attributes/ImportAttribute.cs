using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ImportAttribute : Attribute
    {
        public IEnumerable<Type> Classes { get; }

        public ImportAttribute(params Type[] classes)
        {
            Classes = classes;
        }
    }
}
