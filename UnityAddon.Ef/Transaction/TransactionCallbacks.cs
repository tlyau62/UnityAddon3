using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Ef.Transaction
{
    public interface ITransactionCallbacks
    {
        /*
         * if this method is called inside a transaction, the callback is scheduled to run after tx commit.
         * otherwise, run callback immediately.
         */
        void OnCommit(Action callback);
    }

    public class TransactionCallbacks : ITransactionCallbacks, ITransactionInterceptor
    {
        private AsyncLocal<List<Action>> _callbacks;

        public TransactionCallbacks()
        {
            _callbacks = new AsyncLocal<List<Action>>();
        }

        public void Begin()
        {
            _callbacks.Value = new List<Action>();
        }

        public void Commit()
        {
            _callbacks.Value.ForEach(c =>
            {
                try
                {
                    c();
                }
                catch (Exception e)
                {
                    // TODO log
                }
            });
            _callbacks.Value = null;
        }

        public void Rollback()
        {
            _callbacks.Value = null;
        }

        public void OnCommit(Action callback)
        {
            var callbacks = _callbacks.Value;
            if (callbacks != null)
                _callbacks.Value.Add(callback);
            else
                callback();
        }

    }
}
