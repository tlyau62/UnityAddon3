using Microsoft.EntityFrameworkCore;
using System;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Ef.Transaction
{
    public interface IDbContextTemplate<TDbContext> where TDbContext : DbContext
    {
        TTxResult ExecuteTransaction<TTxResult>(Func<TDbContext, TTxResult> transaction);
        TTxResult ExecuteQuery<TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg);
        TDbContext GetDbContext();
        DbSet<TEntity> GetEntity<TEntity>() where TEntity : class;
    }

    /// <summary>
    /// Used by client to query db.
    /// TODO: logic duplication with TransactionManager
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    [Component]
    public class DbContextTemplate<TDbContext> : IDbContextTemplate<TDbContext> where TDbContext : DbContext
    {
        [Dependency]
        public IDbContextFactory<TDbContext> DbContextFactory { get; set; }

        [OptionalDependency]
        public RollbackOptions RollbackOptions { get; set; }

        public TTxResult ExecuteQuery<TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg)
        {
            return DoInDbContext(ctx =>
            {
                var result = query(ctx);

                AssertNoModifyDbContext(ctx, noModifyMsg);

                return result;
            });
        }

        public TTxResult ExecuteTransaction<TTxResult>(Func<TDbContext, TTxResult> transaction)
        {
            return DoInDbContext(ctx =>
            {
                if (ctx.Database.CurrentTransaction != null)
                {
                    return transaction(ctx);
                }

                var tx = ctx.Database.BeginTransaction();

                try
                {
                    var result = transaction(ctx);

                    if (RollbackOptions != null && RollbackOptions.TestRollback(result))
                    {
                        tx.Rollback();
                    }
                    else
                    {
                        ctx.SaveChanges();
                        tx.Commit();
                    }

                    return result;
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
            });
        }

        public TDbContext GetDbContext()
        {
            return DbContextFactory.Get();
        }

        public DbSet<TEntity> GetEntity<TEntity>() where TEntity : class
        {
            return GetDbContext().Set<TEntity>();
        }

        private void AssertNoModifyDbContext(TDbContext dbContext, string noModifyMsg = "Detected dbcontext is changed by method, but transaction is not opened.")
        {
            if (dbContext.ChangeTracker.HasChanges() && dbContext.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException(noModifyMsg);
            }
        }

        public TTxResult DoInDbContext<TTxResult>(Func<TDbContext, TTxResult> tx)
        {
            var isOpenDbCtx = DbContextFactory.IsOpen();
            var ctx = isOpenDbCtx ? DbContextFactory.Get() : DbContextFactory.Open();

            try
            {
                return tx(ctx);
            }
            finally
            {
                if (!isOpenDbCtx)
                {
                    DbContextFactory.Close();
                }
            }
        }
    }
}