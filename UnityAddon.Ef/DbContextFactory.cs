using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Thread;

namespace UnityAddon.Ef
{
    public interface IDbContextFactory<T>
    {
        /// <summary>
        /// Get the context of current thread 
        /// </summary>
        T Get();

        /// <summary>
        /// Open a new context and bind it to current thread 
        /// </summary>
        /// <returns></returns>
        T Open();

        /// <summary>
        /// true if a opened db connection is binded in current thread
        /// </summary>
        bool IsOpen();

        /// <summary>
        /// Dispose the db context
        /// </summary>
        void Close();
    }

    /// <summary>
    /// A specialized ThreadLocalFactory that manage the construction of a db context object.
    /// The registered DbContext must be scope transient.
    /// </summary>
    [Component]
    public class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        private AsyncLocalFactory<T> _asyncLocalFactory;

        [PostConstruct]
        public void Init()
        {
            _asyncLocalFactory = new AsyncLocalFactory<T>(() => Sp.GetRequiredService<T>());
        }

        public T Get()
        {
            if (!IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is not opened.");
            }

            return _asyncLocalFactory.Get();
        }

        public T Open()
        {
            if (IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is already created.");
            }

            return _asyncLocalFactory.Set();
        }

        public void Close()
        {
            if (!IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is already disposed.");
            }

            _asyncLocalFactory.Delete();
        }

        public bool IsOpen()
        {
            if (!_asyncLocalFactory.Exist())
            {
                return false;
            }

            if (IsDisposed(_asyncLocalFactory.Get()))
            {
                throw new InvalidOperationException("Dbcontext is created, but disposed.");
            }

            return true;
        }

        public bool IsDisposed(T context)
        {
            var ctxType = typeof(DbContext).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance);
            var isDisposed = ctxType.GetValue(context);

            return (bool)isDisposed;
        }
    }

}