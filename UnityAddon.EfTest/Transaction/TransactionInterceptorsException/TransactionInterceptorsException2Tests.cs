using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>), typeof(TransactionInterceptorsTestsConfig<TestTxExceptionInterceptor, TestTxInterceptor>))]
    public class TransactionInterceptorsException2Tests : TransactionInterceptorsCommonExceptionTests<TestTxExceptionInterceptor, TestTxInterceptor>
    {
        [Fact]
        public override void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            base.TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted();

            Assert.Equal("ZCA", LogService.Log);
        }

        [Fact]
        public override void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted()
        {
            base.TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted();

            Assert.Equal("ZDB", LogService.Log);
        }
    }
}
