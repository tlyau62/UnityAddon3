using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using Xunit;

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

    public class TransactionInterceptorsTests : IDisposable
    {
        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public LogService LogService { get; set; }

        public TransactionInterceptorsTests()
        {
            new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace, "UnityAddon.EfTest.Common")
                .ConfigureUA<DbContextTemplateBuilder>(c =>
                {
                    c.AddTransactionInterceptor<TestTxInterceptor>();
                })
                .EnableUnityAddonEf()
                .BuildUA()
                .BuildTestUA(this);

            DbSetupUtility.CreateDb(DbContextFactory);
        }

        [Fact]
        public void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            DbContextTemplate.ExecuteTransaction<TestDbContext, object>(tx =>
            {
                LogService.Log += "Z";

                return null;
            });

            Assert.Equal("AZB", LogService.Log);
        }

        [Fact]
        public void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted()
        {
            Assert.Throws<InvalidOperationException>(() =>
                DbContextTemplate.ExecuteTransaction<TestDbContext, object>(tx =>
                {
                    LogService.Log += "Z";

                    throw new InvalidOperationException();
                }));

            Assert.Equal("AZC", LogService.Log);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
        }
    }
}
