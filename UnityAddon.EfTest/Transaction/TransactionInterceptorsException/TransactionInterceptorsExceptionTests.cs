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

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
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
        }

        public void Commit()
        {
            LogService.Log += "A";
        }

        public void Rollback()
        {
            LogService.Log += "B";

            throw new NullReferenceException();
        }
    }

    public class TestTxExceptionInterceptor : ITransactionInterceptor
    {
        [Dependency]
        public LogService LogService { get; set; }

        public void Begin()
        {
        }

        public void Commit()
        {
            LogService.Log += "C";

            throw new NullReferenceException();
        }

        public void Rollback()
        {
            LogService.Log += "D";
        }
    }

    public class TransactionInterceptorsExceptionTests
    {
        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public LogService LogService { get; set; }

        private void TestBuilder(int normalOrder, int exOrder)
        {
            new HostBuilder()
                   .RegisterUA()
                   .ConfigureContainer<ApplicationContext>(ctx =>
                   {
                       ctx.AddContextEntry(entry => entry.ConfigureBeanDefinitions(defs => defs.AddFromComponentScanner(GetType().Assembly, GetType().Namespace, "UnityAddon.EfTest.Common")));
                       ctx.ConfigureContext<DbContextTemplateOption>(option =>
                       {
                           if (normalOrder < exOrder)
                           {
                               option.AddTransactionInterceptor<TestTxInterceptor>();
                               option.AddTransactionInterceptor<TestTxExceptionInterceptor>();
                           }
                           else
                           {
                               option.AddTransactionInterceptor<TestTxExceptionInterceptor>();
                               option.AddTransactionInterceptor<TestTxInterceptor>();
                           }
                       });
                   })
                   .EnableUnityAddonEf()
                   .Build()
                   .Services
                   .BuildUp(this);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted(int normalOrder, int exOrder)
        {
            TestBuilder(normalOrder, exOrder);

            DbSetupUtility.CreateDb(DbContextFactory);

            DbContextTemplate.ExecuteTransaction<TestDbContext, object>(tx =>
            {
                LogService.Log += "Z";

                return null;
            });

            if (normalOrder < exOrder)
            {
                Assert.Equal("ZAC", LogService.Log);
            }
            else
            {
                Assert.Equal("ZCA", LogService.Log);
            }

            DbSetupUtility.DropDb(DbContextFactory);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted(int normalOrder, int exOrder)
        {
            TestBuilder(normalOrder, exOrder);

            DbSetupUtility.CreateDb(DbContextFactory);

            Assert.Throws<InvalidOperationException>(() => DbContextTemplate.ExecuteTransaction<TestDbContext, object>(tx =>
            {
                LogService.Log += "Z";

                throw new InvalidOperationException();
            }));

            if (normalOrder < exOrder)
            {
                Assert.Equal("ZBD", LogService.Log);
            }
            else
            {
                Assert.Equal("ZDB", LogService.Log);
            }

            DbSetupUtility.DropDb(DbContextFactory);
        }
    }
}
