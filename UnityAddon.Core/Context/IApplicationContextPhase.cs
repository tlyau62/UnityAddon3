using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Context
{
    /// <summary>
    /// Process is invoked before the current phase is started.
    /// </summary>
    public interface IApplicationContextPhase
    {
        void Process();
    }

    public interface IAppCtxPostServiceRegistrationPhase : IApplicationContextPhase
    {
    }

    public interface IAppCtxPreInstantiateSingletonPhase : IApplicationContextPhase
    {
    }

    public interface IAppCtxFinishPhase : IApplicationContextPhase
    {
    }
}
