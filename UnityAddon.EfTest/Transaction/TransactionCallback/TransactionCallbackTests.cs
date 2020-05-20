using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.TransactionCallback
{
    [Component]
    public class Logger
    {
        public string Log = "";
    }

    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>))]
    public class TransactionCallbackTests : UnityAddonEfTest
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        [Fact]
        public void TransactionInterceptorManager_ExecuteTransaction_InterceptorExecuted()
        {
            DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
            {
                DbContextTemplate.RegisterTransactionCallback(() => Logger.Log += "A");

                Logger.Log += "B";

                DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
                {
                    DbContextTemplate.RegisterTransactionCallback(() => Logger.Log += "C");

                    Logger.Log += "D";

                    DbContextTemplate.ExecuteTransaction<TestDbContext>(tx =>
                    {
                        DbContextTemplate.RegisterTransactionCallback(() => Logger.Log += "E");

                        Logger.Log += "F";
                    });
                });
            });

            Assert.Equal("BDFACE", Logger.Log);
        }
    }
}
