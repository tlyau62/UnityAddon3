using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using UnityAddon.Core.Util.ComponentScanning;
using Xunit;
using UnityAddon.Ef.TransactionInterceptor;
using UnityAddon.Test.Attributes;
using UnityAddon.Test;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptors
{
    [Component]
    public class LogService
    {
        public string Log = "";
    }

    public class TestTxInterceptor : ITransactionInterceptor
    {
        [Dependency]
        public LogService LogService { get; set; }

        public void Begin()
        {
            LogService.Log += "A";
        }

        public void Commit()
        {
            LogService.Log += "B";
        }

        public void Rollback()
        {
            LogService.Log += "C";
        }
    }

    [Configuration]
    public class TransactionInterceptorsTestsConfig : UnityAddonEfCustomConfig
    {
        [Bean]
        public override TransactionInterceptorOption TransactionInterceptorOption()
        {
            var option = new TransactionInterceptorOption();

            option.AddTransactionInterceptor<TestTxInterceptor>();

            return option;
        }
    }

    [ComponentScan]
    [Import(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>))]
    public class TransactionInterceptorsTests : UnityAddonEfTest
    {
        public TransactionInterceptorsTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public LogService LogService { get; set; }

        [Fact]
        public void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
            {
                LogService.Log += "Z";
            });

            Assert.Equal("AZB", LogService.Log);
        }

        [Fact]
        public void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted()
        {
            Assert.Throws<InvalidOperationException>(() =>
                DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
                {
                    LogService.Log += "Z";

                    throw new InvalidOperationException();
                }));

            Assert.Equal("AZC", LogService.Log);
        }
    }
}
