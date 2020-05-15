using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Context
{
    /// <summary>
    /// Initialize a bean after all app beans are registered.
    /// </summary>
    public interface IContextPostRegistryInitiable
    {
        void Initialize();
    }
}
