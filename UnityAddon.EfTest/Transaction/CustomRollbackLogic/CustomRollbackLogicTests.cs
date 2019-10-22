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

    public class ConcreteGenericResult<T>
    {
        public bool IsSuccess { get; set; }
    }

    public interface IRepo
    {
        GenericResult<string> AddItemGeneric(bool isSuccess);
        Result AddItem(bool isSuccess);
        ConcreteGenericResult<string> AddStringItemConcreteGeneric(bool isSuccess);
        ConcreteGenericResult<int> AddIntItemConcreteGeneric(bool isSuccess);
    }

    [Component]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        [RequireDbContext(Transactional = true)]
        public GenericResult<string> AddItemGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new GenericResult<string>()
            {
                IsSuccess = isSuccess
            };
        }

        [RequireDbContext(Transactional = true)]
        public Result AddItem(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new Result()
            {
                IsSuccess = isSuccess
            };
        }

        [RequireDbContext(Transactional = true)]
        public ConcreteGenericResult<string> AddStringItemConcreteGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new ConcreteGenericResult<string>()
            {
                IsSuccess = isSuccess
            };
        }

        [RequireDbContext(Transactional = true)]
        public ConcreteGenericResult<int> AddIntItemConcreteGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new ConcreteGenericResult<int>()
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

            rollbackOptions.AddRollbackLogic(typeof(GenericResult<>), returnValue => GetGenericResultValue((dynamic)returnValue));
            rollbackOptions.AddRollbackLogic(typeof(Result), returnValue => GetResultValue((dynamic)returnValue));
            rollbackOptions.AddRollbackLogic(typeof(ConcreteGenericResult<string>), returnValue => GetConcreteGenericResultValue((dynamic)returnValue));

            return rollbackOptions;
        }

        private bool GetGenericResultValue<T>(GenericResult<T> result)
        {
            return !result.IsSuccess;
        }

        private bool GetResultValue(Result result)
        {
            return !result.IsSuccess;
        }

        private bool GetConcreteGenericResultValue(ConcreteGenericResult<string> result)
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
