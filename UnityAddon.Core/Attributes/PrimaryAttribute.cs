using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    /// <summary>
    /// When a type maps to multiple beans and when such type is being resolved,
    /// the primary bean will be resolved. Otherwise, NoUniqueBeanDefinitionException
    /// will be thrown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false)]
    public class PrimaryAttribute : Attribute
    {
    }
}
