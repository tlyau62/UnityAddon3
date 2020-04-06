using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Ef.Transaction
{
    public class DbContextTemplateBuilder
    {
        private IDictionary<Type, List<object>> _rollbackLogics = new Dictionary<Type, List<object>>();

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

        public IDbContextTemplate Build(IUnityContainer container)
        {
            return new DbContextTemplate(_rollbackLogics, container);
        }
    }
}
