using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Exceptions
{
    public class NoSuchBeanDefinitionException : Exception
    {
        public NoSuchBeanDefinitionException(string message) : base(message)
        {
        }
    }
}
