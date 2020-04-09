using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Ef.Transaction
{
    public interface ITransactionManager<T>
    {
        void DoInDbContext(IInvocation invocation, bool transactional);
    }

    /// <summary>
    /// Handle transaction logic.
    /// </summary>
    [Component]
    public class TransactionManager<TDbContext> : ITransactionManager<TDbContext> where TDbContext : DbContext
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        public void DoInDbContext(IInvocation invocation, bool transactional)
        {
            if (transactional)
            {
                DbContextTemplate.ExecuteTransaction<TDbContext, object>(Tx);
            }
            else
            {
                var noModifyMsg = $"Detected dbcontext is changed by method {invocation.MethodInvocationTarget.Name} at class {invocation.MethodInvocationTarget.DeclaringType.FullName}, but transaction is not opened.";

                DbContextTemplate.ExecuteQuery<TDbContext, object>(Tx, noModifyMsg);
            }

            object Tx(TDbContext ctx)
            {
                invocation.Proceed();

                return invocation.ReturnValue;
            }
        }
    }

}
