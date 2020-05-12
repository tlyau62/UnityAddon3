using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// arg[0] = key string
    /// arg[1] = value object
    /// arg[2] = value type
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ConfigArgAttribute : Attribute
    {
        public List<object[]> Args { get; protected set; } = new List<object[]>();

        public ConfigArgAttribute(params object[] args)
        {
            if (args.Length > 0)
            {
                Args.Add(args);
            }
        }
    }
}
