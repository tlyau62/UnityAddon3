using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Thread
{
    public interface IThreadLocalFactory<T>
    {
        T Get();
        T Set();
        bool Exist();
        void Delete();
    }

    /// <summary>
    /// A factory to manage thread local.
    /// </summary>
    public class ThreadLocalFactory<T> : IThreadLocalFactory<T>
    {
        private readonly Func<T> createFunc;
        private ThreadLocal<T> currentContext = new ThreadLocal<T>();

        public ThreadLocalFactory(Func<T> createFunc)
        {
            this.createFunc = createFunc;
        }

        public T Get()
        {
            if (!Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).FullName} is not set.");
            }

            return currentContext.Value;
        }

        public T Set()
        {
            if (Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).FullName} has been created.");
            }

            return currentContext.Value = createFunc();
        }

        public bool Exist()
        {
            return !Equals(currentContext.Value, default(T));
        }

        public void Delete()
        {
            if (!Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).FullName} is not set.");
            }

            if (typeof(T) is IDisposable)
            {
                ((IDisposable)currentContext.Value).Dispose();
            }

            currentContext.Value = default(T);
        }
    }
}
