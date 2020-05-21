using System;
using Unity;
using UnityAddon.Core.Context;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using UnityAddon.Test;
using Xunit;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    public class TransactionInterceptorsCommonExceptionTests<T1, T2> : UnityAddonEfTest
        where T1 : ITransactionInterceptor
        where T2 : ITransactionInterceptor
    {
        public TransactionInterceptorsCommonExceptionTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public LogService LogService { get; set; }

        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        public virtual void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
            {
                LogService.Log += "Z";
            });
        }

        public virtual void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted()
        {
            Assert.Throws<InvalidOperationException>(() => DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
            {
                LogService.Log += "Z";

                throw new InvalidOperationException();
            }));
        }
    }
}
