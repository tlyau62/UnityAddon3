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
            var type = returnValue.GetType();
            var isGenericType = type.IsGenericType;

            if (!_rollbackLogics.ContainsKey(type) &&
                isGenericType && !_rollbackLogics.ContainsKey(type.GetGenericTypeDefinition()))
            {
                return false;
            }

            var rollbackLogic = _rollbackLogics.ContainsKey(type) ? _rollbackLogics[type] : _rollbackLogics[type.GetGenericTypeDefinition()];

            return rollbackLogic.Any(logic => logic(returnValue));
        }
    }
}
