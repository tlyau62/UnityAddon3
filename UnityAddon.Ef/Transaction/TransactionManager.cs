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
    public class TransactionManager<T> : ITransactionManager<T> where T: DbContext
    {
        [Dependency]
        public IDbContextFactory<T> _dbContextFactory { get; set; }

        [OptionalDependency]
        public RollbackOptions RollbackOptions { get; set; }

        // TODO allow multi interceptors, change to List<ITransactionInterceptor>
        [OptionalDependency]
        public ITransactionInterceptor TransactionInterceptor { get; set; }

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
            TransactionInterceptor?.Begin();

            try
            {
                invocation.Proceed();

                if (RollbackOptions != null && RollbackOptions.TestRollback(invocation.ReturnValue))
                {
                    tx.Rollback();
                    TransactionInterceptor?.Rollback();
                }
                else
                {
                    context.SaveChanges();
                    tx.Commit();
                    // TODO try-catch all operations of interceptor,
                    // exception in interceptor should not be thrown out after tx has already committed
                    TransactionInterceptor?.Commit();
                }
            }
            catch (Exception)
            {
                tx.Rollback();
                TransactionInterceptor?.Rollback();
                throw;
            }
            finally
            {
                tx.Dispose();
            }
        }

    }

}
