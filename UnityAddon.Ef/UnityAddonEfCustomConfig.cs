using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.RollbackLogics;
using UnityAddon.Ef.TransactionInterceptor;

namespace UnityAddon.Ef
{
    public abstract class UnityAddonEfCustomConfig
    {
        [Bean]
        public virtual RollbackLogicOption RollbackLogicOption() => null;

        [Bean]
        public virtual Type DataSource()
        {
            return null;
        }

        [Bean]
        public virtual TransactionInterceptorOption TransactionInterceptorOption() => null;
    }
}
