using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// Indicates one or more component classes to import — typically @Configuration classes.
    /// </summary>
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
