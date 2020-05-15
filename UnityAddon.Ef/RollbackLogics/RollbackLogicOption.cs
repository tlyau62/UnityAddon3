using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef.RollbackLogics
{
    public class RollbackLogicOption
    {
        public IDictionary<Type, List<object>> RollbackLogics { get; } = new Dictionary<Type, List<object>>();

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
    }
}
