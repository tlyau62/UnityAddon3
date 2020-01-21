using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef
{
    /// <summary>
    /// This attribute ensures the UnityAddonEf assembly is loaded up at the client application.
    /// Should put attribute at class level. Method level won't be effective.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EnableUnityAddonEfAttribute : Attribute
    {
    }
}
