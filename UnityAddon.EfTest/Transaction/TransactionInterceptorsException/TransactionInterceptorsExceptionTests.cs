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
using Microsoft.Extensions.DependencyInjection;

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

    [ComponentScan(typeof(TransactionInterceptorsExceptionTests))]
    [Import(typeof(UnityAddonEfConfig))]
    [Import(typeof(TestDbConfig<TestDbContext>))]
    public class TransactionInterceptorsExceptionTests : UnityAddonEfTest
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public LogService LogService { get; set; }

        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted(int normalOrder, int exOrder)
        {
            ApplicationContext.ConfigureContext<DbContextTemplateOption>(option =>
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

            DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
            {
                LogService.Log += "Z";
            });

            if (normalOrder < exOrder)
            {
                Assert.Equal("ZAC", LogService.Log);
            }
            else
            {
                Assert.Equal("ZCA", LogService.Log);
            }
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void TransactionInterceptorManager_ExecuteTransactionWithException_InterceptorExecuted(int normalOrder, int exOrder)
        {
            ApplicationContext.ConfigureContext<DbContextTemplateOption>(option =>
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

            Assert.Throws<InvalidOperationException>(() => DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
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
        }
    }
}
