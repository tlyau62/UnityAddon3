using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Ef;

namespace UnityAddon.EF.Transaction
{
    public interface IRequireDbContextHandler
    {
        void DoInDbContext(IInvocation invocation, bool transactional);
    }

    [Component]
    public class RequireDbContextHandler : IRequireDbContextHandler
    {
        [Dependency]
        public IDbContextFactory _dbContextFactory { get; set; }

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
                    _dbContextFactory.Delete();
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

        private void DoInDbContextTx(DbContext context, IInvocation invocation)
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

                context.SaveChanges();
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
            finally
            {
                tx.Dispose();
            }
        }

    }

}
