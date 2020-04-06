using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Ef.Transaction
{
    public interface IDbContextTemplate
    {
        TTxResult ExecuteTransaction<TDbContext, TTxResult>(Func<TDbContext, TTxResult> transaction) where TDbContext : DbContext;
        TTxResult ExecuteQuery<TDbContext, TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg) where TDbContext : DbContext;
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
        DbSet<TEntity> GetEntity<TDbContext, TEntity>() where TDbContext : DbContext where TEntity : class;
        bool TestRollback(object returnValue);
    }

    /// <summary>
    /// Used by client to deal with any db context.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class DbContextTemplate : IDbContextTemplate
    {
        // TODO allow multi interceptors, change to List<ITransactionInterceptor>
        [OptionalDependency]
        public ITransactionInterceptor TransactionInterceptor { get; set; }

        private static MethodInfo LogicInvokerMethod = typeof(DbContextTemplate).GetMethod(nameof(LogicInvoker), BindingFlags.NonPublic | BindingFlags.Instance);

        private IDictionary<Type, List<object>> _rollbackLogics;

        private IUnityContainer _container;

        public DbContextTemplate(IDictionary<Type, List<object>> rollbackLogics, IUnityContainer container)
        {
            _rollbackLogics = rollbackLogics;
            _container = container;
        }

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
                TransactionInterceptor?.Begin();

                try
                {
                    var result = transaction(ctx);

                    if (TestRollback(result))
                    {
                        tx.Rollback();
                        TransactionInterceptor?.Rollback();
                    }
                    else
                    {
                        ctx.SaveChanges();
                        tx.Commit();
                        // TODO try-catch all operations of interceptor,
                        // exception in interceptor should not be thrown out after tx has already committed
                        TransactionInterceptor?.Commit();
                    }

                    return result;
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
            });
        }

        public IDbContextFactory<TDbContext> GetDbContextFactory<TDbContext>() where TDbContext : DbContext
        {
            return _container.ResolveUA<IDbContextFactory<TDbContext>>();
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            return _container.ResolveUA<IDbContextFactory<TDbContext>>().Get();
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

        /// <summary>
        /// Check whether a return value will be rollbacked by the tx.
        /// </summary>
        public bool TestRollback(object returnValue)
        {
            if (returnValue == null)
            {
                return false;
            }

            var type = returnValue.GetType();
            var regType = GetRegisteredType(type) ?? (type.IsGenericType ? GetRegisteredType(type.GetGenericTypeDefinition()) : null);

            if (regType == null)
            {
                return false;
            }

            return _rollbackLogics[regType].Any(logic => (bool)LogicInvokerMethod.MakeGenericMethod(logic.GetType().GetGenericArguments()[0]).Invoke(this, new[] { logic, returnValue }));
        }

        private bool LogicInvoker<T>(Func<T, bool> logic, object returnValue)
        {
            return logic((T)returnValue);
        }

        private Type GetRegisteredType(Type targetType)
        {
            var types = TypeResolver.GetAssignableTypes(targetType);

            foreach (var type in types)
            {
                if (_rollbackLogics.ContainsKey(type))
                {
                    return type;
                }

                if (type.IsGenericType && _rollbackLogics.ContainsKey(type.GetGenericTypeDefinition()))
                {
                    return type.GetGenericTypeDefinition();
                }
            }

            return null;
        }
    }
}