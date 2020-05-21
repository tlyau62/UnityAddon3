using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using UnityAddon.Core.Util.ComponentScanning;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using UnityAddon.Test.Attributes;
using UnityAddon.Test;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>), typeof(TransactionInterceptorsTestsConfig<TestTxInterceptor, TestTxExceptionInterceptor>))]
    public class TransactionInterceptorsExceptionTests : TransactionInterceptorsCommonExceptionTests<TestTxInterceptor, TestTxExceptionInterceptor>
    {
        public TransactionInterceptorsExceptionTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Fact]
        public override void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            base.TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted();

            Assert.Equal("ZAC", LogService.Log);
        }

        [Fact]
        public override void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted()
        {
            base.TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted();

            Assert.Equal("ZBD", LogService.Log);
        }
    }
}
