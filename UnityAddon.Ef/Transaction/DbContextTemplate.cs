using Microsoft.EntityFrameworkCore;
using System;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Ef.Transaction
{
    public interface IDbContextTemplate
    {
        TTxResult ExecuteTransaction<TDbContext, TTxResult>(Func<TDbContext, TTxResult> transaction) where TDbContext : DbContext;
        TTxResult ExecuteQuery<TDbContext, TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg) where TDbContext : DbContext;
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
        DbSet<TEntity> GetEntity<TDbContext, TEntity>() where TDbContext : DbContext where TEntity : class;
    }

    /// <summary>
    /// Used by client to deal with any db context.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    [Component]
    public class DbContextTemplate : IDbContextTemplate
    {
        [OptionalDependency]
        public RollbackOptions RollbackOptions { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public TTxResult ExecuteQuery<TDbContext, TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg) where TDbContext : DbContext
        {
            return DoInDbContext<TDbContext, TTxResult>(ctx =>
            {
                var result = query(ctx);

                AssertNoModifyDbContext(ctx, noModifyMsg);

                return result;
            });
        }

        public TTxResult ExecuteTransaction<TDbContext, TTxResult>(Func<TDbContext, TTxResult> transaction) where TDbContext : DbContext
        {
            return DoInDbContext<TDbContext, TTxResult>(ctx =>
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

        public IDbContextFactory<TDbContext> GetDbContextFactory<TDbContext>() where TDbContext : DbContext
        {
            return UnityContainer.ResolveUA<IDbContextFactory<TDbContext>>();
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            return UnityContainer.ResolveUA<IDbContextFactory<TDbContext>>().Get();
        }

        public DbSet<TEntity> GetEntity<TDbContext, TEntity>() where TDbContext : DbContext where TEntity : class
        {
            return GetDbContext<TDbContext>().Set<TEntity>();
        }

        private void AssertNoModifyDbContext<TDbContext>(TDbContext dbContext, string noModifyMsg = "Detected dbcontext is changed by method, but transaction is not opened.") where TDbContext : DbContext
        {
            if (dbContext.ChangeTracker.HasChanges() && dbContext.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException(noModifyMsg);
            }
        }

        public TTxResult DoInDbContext<TDbContext, TTxResult>(Func<TDbContext, TTxResult> tx) where TDbContext : DbContext
        {
            var dbCtxFactory = GetDbContextFactory<TDbContext>();
            var isOpenDbCtx = dbCtxFactory.IsOpen();
            var ctx = isOpenDbCtx ? dbCtxFactory.Get() : dbCtxFactory.Open();

            try
            {
                return tx(ctx);
            }
            finally
            {
                if (!isOpenDbCtx)
                {
                    dbCtxFactory.Close();
                }
            }
        }
    }
}