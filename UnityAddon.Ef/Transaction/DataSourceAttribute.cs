using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Ef.Transaction
{
    /// <summary>
    /// Specify the global data source used in db context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DataSourceAttribute : Attribute
    {
        public Type Entity { get; set; }

        public DataSourceAttribute(Type entity)
        {
            Entity = entity;
        }
    }
}
