﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Core.Reflection;
using UnityAddon.Ef.RollbackLogics;
using UnityAddon.Ef.TransactionInterceptor;

namespace UnityAddon.Ef.Transaction
{
    public interface IDbContextTemplate
    {
        TTxResult ExecuteTransaction<TDbContext, TTxResult>(Func<TDbContext, TTxResult> transaction) where TDbContext : DbContext;
        void ExecuteTransaction<TDbContext>(Action<TDbContext> transaction) where TDbContext : DbContext;
        TTxResult ExecuteQuery<TDbContext, TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg = null) where TDbContext : DbContext;
        void ExecuteQuery<TDbContext>(Action<TDbContext> query, string noModifyMsg = null) where TDbContext : DbContext;
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
        DbSet<TEntity> GetEntity<TDbContext, TEntity>() where TDbContext : DbContext where TEntity : class;
        bool TestRollback(object returnValue);
        void RegisterTransactionCallback(Action callback);
    }

    /// <summary>
    /// Used by client to deal with any db context.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    [Component]
    public class DbContextTemplate : IDbContextTemplate, IAppCtxPreInstantiateSingletonPhase
    {
        private static readonly MethodInfo LogicInvokerMethod = typeof(DbContextTemplate).GetMethod(nameof(LogicInvoker), BindingFlags.NonPublic | BindingFlags.Instance);

        private IDictionary<Type, List<object>> _rollbackLogics;

        [OptionalDependency]
        public RollbackLogicOption RollbackLogicOption { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public ITransactionCallbacks TransactionCallbacks { get; set; }

        [Dependency]
        public TransactionInterceptorManager TransactionInterceptorManager { get; set; }

        public TTxResult ExecuteQuery<TDbContext, TTxResult>(Func<TDbContext, TTxResult> query, string noModifyMsg) where TDbContext : DbContext
        {
            return DoInDbContext<TDbContext, TTxResult>(ctx =>
            {
                var result = query(ctx);

                AssertNoModifyDbContext(ctx, noModifyMsg ?? "Detected dbcontext is changed by method, but transaction is not opened.");

                return result;
            });
        }

        public void ExecuteQuery<TDbContext>(Action<TDbContext> query, string noModifyMsg) where TDbContext : DbContext
        {
            ExecuteQuery<TDbContext, object>(ctx =>
            {
                query(ctx);
                return null;
            }, noModifyMsg);
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

                TransactionInterceptorManager.ExecuteBeginCallbacks();

                try
                {
                    var result = transaction(ctx);

                    if (TestRollback(result))
                    {
                        tx.Rollback();
                        TransactionInterceptorManager.ExecuteRollbackCallbacks();
                    }
                    else
                    {
                        ctx.SaveChanges();
                        tx.Commit();
                        TransactionInterceptorManager.ExecuteCommitCallbacks();
                    }

                    return result;
                }
                catch (Exception)
                {
                    tx.Rollback();
                    TransactionInterceptorManager.ExecuteRollbackCallbacks();
                    throw;
                }
                finally
                {
                    tx.Dispose();
                }
            });
        }

        public void ExecuteTransaction<TDbContext>(Action<TDbContext> transaction) where TDbContext : DbContext
        {
            ExecuteTransaction<TDbContext, object>(ctx =>
            {
                transaction(ctx);
                return null;
            });
        }

        public IDbContextFactory<TDbContext> GetDbContextFactory<TDbContext>() where TDbContext : DbContext
        {
            return Sp.GetRequiredService<IDbContextFactory<TDbContext>>();
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            return Sp.GetRequiredService<IDbContextFactory<TDbContext>>().Get();
        }

        public DbSet<TEntity> GetEntity<TDbContext, TEntity>() where TDbContext : DbContext where TEntity : class
        {
            return GetDbContext<TDbContext>().Set<TEntity>();
        }

        private void AssertNoModifyDbContext<TDbContext>(TDbContext dbContext, string noModifyMsg) where TDbContext : DbContext
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

        public void RegisterTransactionCallback(Action callback)
        {
            TransactionCallbacks.OnCommit(callback);
        }

        public void Process()
        {
            _rollbackLogics = RollbackLogicOption == null ? new Dictionary<Type, List<object>>() : RollbackLogicOption.RollbackLogics;
        }
    }
}