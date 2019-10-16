using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    public class NoSuchBeanDefinitionException : Exception
    {
        public NoSuchBeanDefinitionException(string message) : base(message)
        {
        }
    }
}
