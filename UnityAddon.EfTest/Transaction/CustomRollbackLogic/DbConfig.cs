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
            rollbackOptions.RegisterRollbackLogic(typeof(GenericResult<>), returnValue => GetGenericResultValue((dynamic)returnValue));

            // rollback depends on Result
            rollbackOptions.RegisterRollbackLogic<Result>(returnValue => !((TestResult)returnValue).IsSuccess);

            // rollback depends on ConcreteGenericResult<string> only
            rollbackOptions.RegisterRollbackLogic<ConcreteGenericResult<string>>(returnValue => !returnValue.IsSuccess);

            return rollbackOptions;
        }

        private bool GetGenericResultValue<T>(GenericResult<T> result)
        {
            return !result.IsSuccess;
        }
    }
}
