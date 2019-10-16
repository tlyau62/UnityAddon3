using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Thread;

namespace UnityAddon.Ef
{
    public interface IDbContextFactory
    {
        /// <summary>
        /// Get the context of current thread 
        /// </summary>
        DbContext Get();

        /// <summary>
        /// Open a new context and bind it to current thread 
        /// </summary>
        /// <returns></returns>
        DbContext Open();

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
    public class DbContextFactory : IDbContextFactory
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        private ThreadLocalFactory<DbContext> _threadLocalFactory;

        [PostConstruct]
        public void Init()
        {
            _threadLocalFactory = new ThreadLocalFactory<DbContext>(() => ContainerRegistry.Resolve<DbContext>());
        }

        public DbContext Get()
        {
            if (!IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is not opened.");
            }

            return _threadLocalFactory.Get();
        }

        public DbContext Open()
        {
            if (IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is already created.");
            }

            return _threadLocalFactory.Set();
        }

        public void Close()
        {
            if (!IsOpen())
            {
                throw new InvalidOperationException("Dbcontext is already disposed.");
            }

            _threadLocalFactory.Delete();
        }

        public bool IsOpen()
        {
            if (!_threadLocalFactory.Exist())
            {
                return false;
            }

            if (IsDisposed(_threadLocalFactory.Get()))
            {
                throw new InvalidOperationException("Dbcontext is created, but disposed.");
            }

            return true;
        }

        public bool IsDisposed(DbContext context)
        {
            var ctxType = typeof(DbContext).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance);
            var isDisposed = ctxType.GetValue(context);

            return (bool)isDisposed;
        }
    }

}