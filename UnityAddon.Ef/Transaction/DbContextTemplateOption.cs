using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;

namespace UnityAddon.Ef.Transaction
{
    public class DbContextTemplateOption
    {
        public IDictionary<Type, List<object>> RollbackLogics { get; } = new Dictionary<Type, List<object>>();

        public IList<Type> TxInterceptors { get; } = new List<Type>();

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
            if (!RollbackLogics.ContainsKey(returnType))
            {
                RollbackLogics[returnType] = new List<object>();
            }

            RollbackLogics[returnType].Add(rollbackLogic);
        }

        public void AddTransactionInterceptor<TTransactionInterceptor>() where TTransactionInterceptor : ITransactionInterceptor
        {
            TxInterceptors.Add(typeof(TTransactionInterceptor));
        }
    }
}
