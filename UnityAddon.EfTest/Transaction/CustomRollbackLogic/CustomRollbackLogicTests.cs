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
    public class GenericResult<T>
    {
        public bool IsSuccess { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
    }

    public interface IRepo
    {
        GenericResult<string> AddItem(bool isRollback);
    }

    [Component]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        [RequireDbContext(Transactional = true)]
        public GenericResult<string> AddItem(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new GenericResult<string>()
            {
                IsSuccess = isSuccess
            };
        }
    }

    [Configuration]
    public class DbConfig
    {
        [Bean]
        public virtual RollbackOptions RollbackOptions()
        {
            var rollbackOptions = new RollbackOptions();

            rollbackOptions.AddRollbackLogic(typeof(GenericResult<>), returnValue => GetActionResultValue((dynamic)returnValue));

            return rollbackOptions;
        }

        private bool GetActionResultValue<T>(GenericResult<T> result)
        {
            return !result.IsSuccess;
        }
    }

    [Trait("Transaction", "RollbackOptions")]
    public class CustomRollbackLogicTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory _dbContextFactory;
        private IRepo _repo;
        private DbSet<Item> _items => ((TestDbContext)(_dbContextFactory.IsOpen() ? _dbContextFactory.Get() : _dbContextFactory.Open())).Items;

        public CustomRollbackLogicTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory>();
            _repo = _appContext.Resolve<IRepo>();

            CreateDb();
        }

        public void Dispose()
        {
            DropDb();
        }

        [Fact]
        public void RollbackOptions_RollbackOnCustomRollbackLogics_DbRollbacked()
        {
            _repo.AddItem(false);

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(0, _items.Count());
        }

        [Fact]
        public void RollbackOptions_RollbackOnCustomRollbackLogics_DbNotRollbacked()
        {
            _repo.AddItem(true);

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
