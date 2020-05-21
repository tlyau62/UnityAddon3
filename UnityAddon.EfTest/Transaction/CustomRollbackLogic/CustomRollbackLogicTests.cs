using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
using UnityAddon.Ef.RollbackLogics;
using UnityAddon.Test.Attributes;
using UnityAddon.Test;

namespace UnityAddon.EfTest.Transaction.CustomRollbackLogic
{
    [Configuration]
    public class CustomRollbackLogicConfig : UnityAddonEfCustomConfig
    {
        public override RollbackLogicOption RollbackLogicOption()
        {
            var option = new RollbackLogicOption();

            // rollback depends on GenericResult<T> any type T
            option.RegisterRollbackLogic(typeof(GenericResult<>), returnValue => !((dynamic)returnValue).IsSuccess);

            // rollback depends on Result
            option.RegisterRollbackLogic<Result>(returnValue => !((TestResult)returnValue).IsSuccess);

            // rollback depends on ConcreteGenericResult<string> only
            option.RegisterRollbackLogic<ConcreteGenericResult<string>>(returnValue => !returnValue.IsSuccess);

            return option;
        }
    }

    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>))]
    public class CustomRollbackLogicTests : UnityAddonEfTest
    {
        public CustomRollbackLogicTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IRepo Repo { get; set; }

        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

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
