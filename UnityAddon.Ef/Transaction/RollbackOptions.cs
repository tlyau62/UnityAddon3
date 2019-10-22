using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityAddon.Ef.Transaction
{
    public class RollbackOptions
    {
        private IDictionary<Type, List<Func<object, bool>>> _rollbackLogics = new Dictionary<Type, List<Func<object, bool>>>();

        public void AddRollbackLogic(Type returnType, Func<object, bool> rollbackLogic)
        {
            if (!_rollbackLogics.ContainsKey(returnType.GetType()))
            {
                _rollbackLogics[returnType] = new List<Func<object, bool>>();
            }

            _rollbackLogics[returnType].Add(rollbackLogic);
        }

        public bool TestRollback(object returnValue)
        {
            var type = returnValue.GetType().IsGenericType ? returnValue.GetType().GetGenericTypeDefinition() : returnValue.GetType();

            if (!_rollbackLogics.ContainsKey(type))
            {
                return false;
            }

            return _rollbackLogics[type].Any(logic => logic(returnValue));
        }
    }
}
