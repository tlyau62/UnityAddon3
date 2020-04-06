using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace UnityAddon.Ef.Transaction
{
    public class DbContextTemplateBuilder
    {
        private readonly IDictionary<Type, List<object>> _rollbackLogics = new Dictionary<Type, List<object>>();

        private readonly IList<Type> _txInterceptors = new List<Type>();

        public void RegisterRollbackLogic<TReturn>(Func<TReturn, bool> rollbackLogic)
        {
            AddRollbackLogic(typeof(TReturn), rollbackLogic);
        }

        public void RegisterRollbackLogic(Type returnType, Func<object, bool> rollbackLogic)
        {
            AddRollbackLogic(returnType, rollbackLogic);
        }

        private void AddRollbackLogic(Type returnType, object rollbackLogic)
        {
            if (!_rollbackLogics.ContainsKey(returnType))
            {
                _rollbackLogics[returnType] = new List<object>();
            }

            _rollbackLogics[returnType].Add(rollbackLogic);
        }

        public void AddTransactionInterceptor<TTransactionInterceptor>() where TTransactionInterceptor : ITransactionInterceptor
        {
            _txInterceptors.Add(typeof(TTransactionInterceptor));
        }

        public IDbContextTemplate Build(IUnityContainer container)
        {
            var txInterceptors = _txInterceptors.Select(t => (ITransactionInterceptor)container.Resolve(t));

            return new DbContextTemplate(_rollbackLogics, container, new TransactionInterceptorManager(txInterceptors));
        }
    }
}
