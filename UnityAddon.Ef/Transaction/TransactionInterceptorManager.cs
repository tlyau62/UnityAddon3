using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Ef.Transaction
{
    public class TransactionInterceptorManager
    {
        private readonly IEnumerable<ITransactionInterceptor> _txInterceptors;

        private readonly ILogger _logger = Log.ForContext<TransactionInterceptorManager>();

        public TransactionInterceptorManager(IEnumerable<ITransactionInterceptor> txInterceptors)
        {
            _txInterceptors = txInterceptors;
        }

        public void ExecuteBeginCallbacks()
        {
            foreach (var itr in _txInterceptors)
            {
                itr.Begin();
            }
        }

        public void ExecuteCommitCallbacks()
        {
            foreach (var itr in _txInterceptors)
            {
                try
                {
                    itr.Commit();
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, $"Transaction interceptor '{itr.GetType()}' throws an exception.");
                }
            }
        }

        public void ExecuteRollbackCallbacks()
        {
            foreach (var itr in _txInterceptors)
            {
                itr.Rollback();
            }
        }
    }
}
