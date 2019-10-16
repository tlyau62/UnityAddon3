using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}
