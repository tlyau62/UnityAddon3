using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.EfTest.Transaction.CustomRollbackLogic
{
    public class GenericResult<T>
    {
        public bool IsSuccess { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
    }

    public class ConcreteGenericResult<T>
    {
        public bool IsSuccess { get; set; }
    }
}
