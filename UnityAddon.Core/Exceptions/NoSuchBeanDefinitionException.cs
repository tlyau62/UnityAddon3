using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Throw if a bean type is not found, but referenced.
    /// </summary>
    public class NoSuchBeanDefinitionException : Exception
    {
        public NoSuchBeanDefinitionException(string message) : base(message)
        {
        }
    }
}
