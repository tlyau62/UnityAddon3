using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}
