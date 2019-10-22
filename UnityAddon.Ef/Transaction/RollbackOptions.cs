using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Reflection;

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
            if (returnValue == null)
            {
                return false;
            }

            var type = returnValue.GetType();
            var regType = GetRegisteredType(type) ?? (type.IsGenericType ? GetRegisteredType(type.GetGenericTypeDefinition()) : null);

            if (regType == null)
            {
                return false;
            }

            return _rollbackLogics[regType].Any(logic => logic(returnValue));
        }

        private Type GetRegisteredType(Type targetType)
        {
            var types = TypeHierarchyScanner.GetAssignableTypes(targetType);

            foreach (var type in types)
            {
                if (_rollbackLogics.ContainsKey(type))
                {
                    return type;
                }
            }

            return null;
        }
    }
}
