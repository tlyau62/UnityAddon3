using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef.Transaction
{
    public interface ITransactionInterceptor
    {
        void Begin();
        void Commit();
        void Rollback();
    }
}
