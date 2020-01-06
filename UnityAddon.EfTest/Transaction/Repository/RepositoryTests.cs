using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.Repository
{
    [Trait("Transaction", "Repository")]
    public class RepositoryTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory<TestDbContext> _dbContextFactory;
        private IRepo _repo;

        public RepositoryTests()
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

        [Fact]
        public void RequireDbContextHandler_QueryItem_ResultReceived()
        {
            Assert.Equal(0, _repo.CountItem());

            Assert.False(_dbContextFactory.IsOpen());
        }

        [Fact]
        public void RequireDbContextHandler_ModifyDbWithoutTransaction_ExceptionThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _repo.InsertItem(new Item("testitem")));

            Assert.Equal($"Detected dbcontext is changed by method InsertItem at class {typeof(Repo).FullName}, but transaction is not opened.", ex.Message);
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
