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
    public interface IRequireDbContextHandler<T>
    {
        void DoInDbContext(IInvocation invocation, bool transactional);
    }

    /// <summary>
    /// Handle transaction logic.
    /// </summary>
    [Component]
    public class RequireDbContextHandler<T> : IRequireDbContextHandler<T> where T: DbContext
    {
        [Dependency]
        public IDbContextFactory<T> _dbContextFactory { get; set; }

        [OptionalDependency]
        public RollbackOptions RollbackOptions { get; set; }

        public void DoInDbContext(IInvocation invocation, bool transactional)
        {
            var isOpen = _dbContextFactory.IsOpen();

            if (!isOpen)
            {
                _dbContextFactory.Open();
            }

            try
            {
                Proceed(invocation, transactional);
            }
            finally
            {
                if (!isOpen)
                {
                    _dbContextFactory.Close();
                }
            }
        }

        private void Proceed(IInvocation invocation, bool transactional)
        {
            if (transactional)
            {
                DoInDbContextTx(_dbContextFactory.Get(), invocation);
            }
            else
            {
                invocation.Proceed();
                AssertNoModifyDbContext(invocation);
            }
        }

        private void AssertNoModifyDbContext(IInvocation invocation)
        {
            var dbContext = _dbContextFactory.Get();

            if (dbContext.ChangeTracker.HasChanges() && dbContext.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException($"Detected dbcontext is changed by method {invocation.MethodInvocationTarget.Name} at class {invocation.MethodInvocationTarget.DeclaringType.FullName}, but transaction is not opened.");
            }
        }

        private void DoInDbContextTx(T context, IInvocation invocation)
        {
            if (context.Database.CurrentTransaction != null)
            {
                invocation.Proceed();

                return;
            }

            var tx = context.Database.BeginTransaction();

            try
            {
                invocation.Proceed();

                if (RollbackOptions != null && RollbackOptions.TestRollback(invocation.ReturnValue))
                {
                    tx.Rollback();
                }
                else
                {
                    context.SaveChanges();
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                tx.Dispose();
            }
        }

    }

}
