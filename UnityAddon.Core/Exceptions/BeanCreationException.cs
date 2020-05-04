using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    public class BeanCreationException : Exception
    {
        public BeanCreationException(string message) : base(message)
        {
        }
    }
}
