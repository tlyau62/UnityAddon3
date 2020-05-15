using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Ef.TransactionInterceptor;

namespace UnityAddon.Ef.Transaction
{
    [Component]
    [Scope(ScopeType.Transient)]
    public class TransactionInterceptorManager
    {
        private readonly ILogger _logger = Log.ForContext<TransactionInterceptorManager>();

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IServicePostRegistry PostRegistry { get; set; }

        [Dependency]
        public IEnumerable<ITransactionInterceptor> TxInterceptors { get; set; }

        public void ExecuteBeginCallbacks()
        {
            foreach (var itr in TxInterceptors)
            {
                itr.Begin();
            }
        }

        public void ExecuteCommitCallbacks()
        {
            foreach (var itr in TxInterceptors)
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
            foreach (var itr in TxInterceptors)
            {
                try
                {
                    itr.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, $"Transaction interceptor '{itr.GetType()}' throws an exception.");
                }
            }
        }
    }
}
