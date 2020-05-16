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
    public class TransactionInterceptorManager : IContextPostRegistryInitiable
    {
        private readonly ILogger _logger = Log.ForContext<TransactionInterceptorManager>();

        private IEnumerable<ITransactionInterceptor> _txInterceptors;

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IServicePostRegistry PostRegistry { get; set; }

        [OptionalDependency]
        public TransactionInterceptorOption TransactionInterceptorOption { get; set; }

        [Dependency]
        public IServicePostRegistry ServicePostRegistry { get; set; }

        public void Initialize()
        {
            if (TransactionInterceptorOption != null)
            {
                foreach (var itctType in TransactionInterceptorOption.TxInterceptors)
                {
                    ServicePostRegistry.AddSingleton(typeof(ITransactionInterceptor), itctType, null);
                }
            }

            _txInterceptors = Sp.GetServices<ITransactionInterceptor>();
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
