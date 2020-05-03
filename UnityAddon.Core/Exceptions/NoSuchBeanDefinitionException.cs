using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Throw if a bean definition is not found, but referenced.
    /// </summary>
    public class NoSuchBeanDefinitionException : Exception
    {
        public NoSuchBeanDefinitionException()
        {
        }

        public NoSuchBeanDefinitionException(string message) : base(message)
        {
        }

        public NoSuchBeanDefinitionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
