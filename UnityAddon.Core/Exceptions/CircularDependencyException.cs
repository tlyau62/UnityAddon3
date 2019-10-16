using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Throw if circular dependency happens
    /// </summary>
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}
