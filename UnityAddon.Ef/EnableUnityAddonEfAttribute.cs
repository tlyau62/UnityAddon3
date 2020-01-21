using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef
{
    /// <summary>
    /// This attribute ensures the UnityAddonEf assembly is loaded up at the client application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EnableUnityAddonEfAttribute : Attribute
    {
    }
}
