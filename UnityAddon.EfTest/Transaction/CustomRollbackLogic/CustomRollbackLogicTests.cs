using Microsoft.EntityFrameworkCore;
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
    public class CustomRollbackLogicTests : EfDefaultTest<TestDbContext>
    {
        [Dependency]
        public IRepo Repo;

        private DbSet<Item> _items => (DbContextFactory.IsOpen() ? DbContextFactory.Get() : DbContextFactory.Open()).Items;

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
