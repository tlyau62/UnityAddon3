using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Ef.Transaction
{
    public class RollbackOptions
    {
        private IDictionary<Type, List<object>> _rollbackLogics = new Dictionary<Type, List<object>>();

        private static MethodInfo LogicInvokerMethod = typeof(RollbackOptions).GetMethod(nameof(LogicInvoker), BindingFlags.NonPublic | BindingFlags.Instance);

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

            return _rollbackLogics[regType].Any(logic => (bool)LogicInvokerMethod.MakeGenericMethod(logic.GetType().GetGenericArguments()[0]).Invoke(this, new[] { logic, returnValue }));
        }

        private bool LogicInvoker<T>(Func<T, bool> logic, object returnValue)
        {
            return logic((T)returnValue);
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
