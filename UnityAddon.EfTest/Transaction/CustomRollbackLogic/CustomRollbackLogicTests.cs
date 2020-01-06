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
    public class CustomRollbackLogicTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory<TestDbContext> _dbContextFactory;
        private IRepo _repo;
        private DbSet<Item> _items => (_dbContextFactory.IsOpen() ? _dbContextFactory.Get() : _dbContextFactory.Open()).Items;

        public CustomRollbackLogicTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory<TestDbContext>>();
            _repo = _appContext.Resolve<IRepo>();

            CreateDb();
        }

        public void Dispose()
        {
            DropDb();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnGenericReturnValue(bool isSuccessResult)
        {
            _repo.AddItemGeneric(isSuccessResult);

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnReturnValue(bool isSuccessResult)
        {
            _repo.AddItem(isSuccessResult);

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptions_RollbackOnConcreteGenericReturnValue(bool isSuccessResult)
        {
            _repo.AddStringItemConcreteGeneric(isSuccessResult);

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(isSuccessResult ? 1 : 0, _items.Count());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RequireDbContextHandler_RollbackOptionsOnNotRegisteredReturnType_NoRollbacked(bool isSuccessResult)
        {
            _repo.AddIntItemConcreteGeneric(isSuccessResult);

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(1, _items.Count());
        }

        private void CreateDb()
        {
            if (!_dbContextFactory.IsOpen())
            {
                _dbContextFactory.Open();
            }

            _dbContextFactory.Get().Database.EnsureCreated();
            _dbContextFactory.Close();
        }

        private void DropDb()
        {
            if (!_dbContextFactory.IsOpen())
            {
                _dbContextFactory.Open();
            }

            _dbContextFactory.Get().Database.EnsureDeleted();
            _dbContextFactory.Close();
        }
    }
}
