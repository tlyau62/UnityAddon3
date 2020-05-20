using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Context
{
    public interface IContextPostInstantiateSingleton
    {
        void PostInitialize();
    }
}
