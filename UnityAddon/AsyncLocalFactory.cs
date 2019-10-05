using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityAddon
{
    public interface IAsyncLocalFactory<T>
    {
        T Get();
        T Set();
        bool Exist();
        void Delete();
    }

    public class AsyncLocalFactory<T> : IAsyncLocalFactory<T>
    {
        private readonly Func<T> createFunc;
        private AsyncLocal<T> currentContext = new AsyncLocal<T>();

        public AsyncLocalFactory(Func<T> createFunc)
        {
            this.createFunc = createFunc;
        }

        public T Get()
        {
            if (!Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).GetType()} is not set.");
            }

            return currentContext.Value;
        }

        public T Set()
        {
            if (Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).GetType()} has been created.");
            }

            return currentContext.Value = createFunc();
        }

        public bool Exist()
        {
            return currentContext.Value != null;
        }

        public void Delete()
        {
            if (!Exist())
            {
                throw new InvalidOperationException($"Instance for {typeof(T).GetType()} is not set.");
            }

            currentContext.Value = default(T);
        }
    }
}
