using System;
using Unity;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    public class TestTxExceptionInterceptor : ITransactionInterceptor
    {
        [Dependency]
        public LogService LogService { get; set; }

        public void Begin()
        {
        }

        public void Commit()
        {
            LogService.Log += "C";

            throw new NullReferenceException();
        }

        public void Rollback()
        {
            LogService.Log += "D";
        }
    }
}
