using System;
using Unity;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    public class TestTxInterceptor : ITransactionInterceptor
    {
        [Dependency]
        public LogService LogService { get; set; }

        public void Begin()
        {
        }

        public void Commit()
        {
            LogService.Log += "A";
        }

        public void Rollback()
        {
            LogService.Log += "B";

            throw new NullReferenceException();
        }
    }
}
