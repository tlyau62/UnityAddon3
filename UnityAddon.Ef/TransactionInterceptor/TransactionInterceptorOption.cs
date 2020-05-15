using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.Ef.TransactionInterceptor
{
    public class TransactionInterceptorOption
    {
        public IList<Type> TxInterceptors { get; } = new List<Type>();

        public void AddTransactionInterceptor<TTransactionInterceptor>() where TTransactionInterceptor : ITransactionInterceptor
        {
            TxInterceptors.Add(typeof(TTransactionInterceptor));
        }
    }
}
