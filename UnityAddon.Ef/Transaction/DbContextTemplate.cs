using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Ef;

namespace UnityAddon.EF.Transaction
{
    public interface IDbContextTemplate<T> where T : DbContext
    {
        void ExecuteTransaction(Action<T> transaction);
        T GetDbContext();
        DbSet<TEntity> GetEntity<TEntity>() where TEntity : class;
    }

    [Component]
    public class DbContextTemplate<T> : IDbContextTemplate<T> where T : DbContext
    {
        [Dependency]
        public IDbContextFactory DbContextFactory { get; set; }

        public void ExecuteTransaction(Action<T> transaction)
        {
            var isOpenDbCtx = DbContextFactory.IsOpen();
            T ctx = (T)(isOpenDbCtx ? DbContextFactory.Get() : DbContextFactory.Open());
            var tx = ctx.Database.BeginTransaction();

            try
            {
                transaction(ctx);

                ctx.SaveChanges();
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

                if (!isOpenDbCtx)
                {
                    DbContextFactory.Close();
                }
            }
        }

        public T GetDbContext()
        {
            return (T)DbContextFactory.Get();
        }

        public DbSet<TEntity> GetEntity<TEntity>() where TEntity : class
        {
            return GetDbContext().Set<TEntity>();
        }
    }
}