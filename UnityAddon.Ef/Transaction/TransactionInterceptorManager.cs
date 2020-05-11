using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.Config;

namespace UnityAddon.Ef.Transaction
{
    [Component]
    public class TransactionInterceptorManager
    {
        private readonly ILogger _logger = Log.ForContext<TransactionInterceptorManager>();

        private IEnumerable<ITransactionInterceptor> _txInterceptors;

        [Dependency]
        public IConfigs<DbContextTemplateOption> DbCtxTemplateOption { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IServicePostRegistry PostRegistry { get; set; }

        [PostConstruct]
        public void Init()
        {
            foreach (var itctType in DbCtxTemplateOption.Value.TxInterceptors)
            {
                PostRegistry.AddSingleton(typeof(ITransactionInterceptor), itctType, null);
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
