using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.CustomRollbackLogic
{
    [Trait("Transaction", "CustomRollbackLogic")]
    public class CustomRollbackLogicTests : IDisposable
    {
        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IRepo Repo;

        private DbSet<Item> _items => (DbContextFactory.IsOpen() ? DbContextFactory.Get() : DbContextFactory.Open()).Items;

        public CustomRollbackLogicTests()
        {
            new HostBuilder()
                   .RegisterUA()
                   .ScanComponentsUA(GetType().Namespace, "UnityAddon.EfTest.Common")
                   .ConfigureUA<DbContextTemplateBuilder>(c =>
                   {
                       // rollback depends on GenericResult<T> any type T
                       c.RegisterRollbackLogic(typeof(GenericResult<>), returnValue => !((dynamic)returnValue).IsSuccess);

                       // rollback depends on Result
                       c.RegisterRollbackLogic<Result>(returnValue => !((TestResult)returnValue).IsSuccess);

                       // rollback depends on ConcreteGenericResult<string> only
                       c.RegisterRollbackLogic<ConcreteGenericResult<string>>(returnValue => !returnValue.IsSuccess);
                   })
                   .EnableUnityAddonEf()
                   .BuildUA()
                   .BuildTestUA(this);

            DbSetupUtility.CreateDb(DbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnGenericReturnValue(bool isSuccessResult)
        {
            Repo.AddItemGeneric(isSuccessResult);

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnReturnValue(bool isSuccessResult)
        {
            Repo.AddItem(isSuccessResult);

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnConcreteGenericReturnValue(bool isSuccessResult)
        {
            Repo.AddStringItemConcreteGeneric(isSuccessResult);

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptionsOnNotRegisteredReturnType_NoRollbacked(bool isSuccessResult)
        {
            Repo.AddIntItemConcreteGeneric(isSuccessResult);

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(1, _items.Count());
        }
    }
}
