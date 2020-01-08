using Microsoft.EntityFrameworkCore;
using System;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Ef.Transaction
{
    public interface IDbContextTemplate<T> where T : DbContext
    {
        void ExecuteTransaction(Action<T> transaction);
        T GetDbContext();
        DbSet<TEntity> GetEntity<TEntity>() where TEntity : class;
    }

    /// <summary>
    /// Used by client to query db.
    /// TODO: logic duplication with TransactionManager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Component]
    public class DbContextTemplate<T> : IDbContextTemplate<T> where T : DbContext
    {
        [Dependency]
        public IDbContextFactory<T> DbContextFactory { get; set; }

        public void ExecuteTransaction(Action<T> transaction)
        {
            var isOpenDbCtx = DbContextFactory.IsOpen();
            T ctx = isOpenDbCtx ? DbContextFactory.Get() : DbContextFactory.Open();
            var tx = ctx.Database.BeginTransaction();

            try
            {
                transaction(ctx);

                ctx.SaveChanges();
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                tx.Dispose();

                if (!isOpenDbCtx)
                {
                    DbContextFactory.Close();
                }
            }
        }

        public T GetDbContext()
        {
            return DbContextFactory.Get();
        }

        public DbSet<TEntity> GetEntity<TEntity>() where TEntity : class
        {
            return GetDbContext().Set<TEntity>();
        }
    }
}