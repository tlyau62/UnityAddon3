using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;

namespace UnityAddon.EfTest.Transaction.CustomRollbackLogic
{
    [Configuration]
    public class DbConfig
    {
        [Bean]
        public virtual RollbackOptions RollbackOptions()
        {
            var rollbackOptions = new RollbackOptions();

            // rollback depends on GenericResult<T> any type T
            rollbackOptions.AddRollbackLogic(typeof(GenericResult<>), returnValue => GetGenericResultValue((dynamic)returnValue));

            // rollback depends on Result
            rollbackOptions.AddRollbackLogic(typeof(Result), returnValue => GetResultValue((dynamic)returnValue));

            // rollback depends on ConcreteGenericResult<string> only
            rollbackOptions.AddRollbackLogic(typeof(ConcreteGenericResult<string>), returnValue => GetConcreteGenericResultValue((dynamic)returnValue));

            return rollbackOptions;
        }

        private bool GetGenericResultValue<T>(GenericResult<T> result)
        {
            return !result.IsSuccess;
        }

        private bool GetResultValue(TestResult result)
        {
            return !result.IsSuccess;
        }

        private bool GetConcreteGenericResultValue(ConcreteGenericResult<string> result)
        {
            return !result.IsSuccess;
        }
    }
}
