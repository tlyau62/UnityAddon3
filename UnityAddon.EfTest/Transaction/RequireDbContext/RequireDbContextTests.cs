using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.RequireDbContext
{
    [Trait("Transaction", "RequireDbContext")]
    public class RequireDbContextTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory<TestDbContext> _dbContextFactory;
        private IRepoA _repoA;
        private IRepoB _repoB;
        private DbSet<Item> _items => (_dbContextFactory.IsOpen() ? _dbContextFactory.Get() : _dbContextFactory.Open()).Items;

        public RequireDbContextTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory<TestDbContext>>();
            _repoA = _appContext.Resolve<IRepoA>();
            _repoB = _appContext.Resolve<IRepoB>();

            DbSetupUtility.CreateDb(_dbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(_dbContextFactory);
        }

        [Fact]
        public void RequireDbContextHandler_AddItem_ItemCreated()
        {
            _repoB.AddItem();

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal("testitem", _items.Single().Name);
        }

        [Fact]
        public void RequireDbContextHandler_ExceptionThrownInTransaction_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => _repoB.AddItemException());

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Empty(_items);
        }

        [Fact]
        public void RequireDbContextHandler_NestedCallOnRequireDbMethod_ItemCreated()
        {
            _repoA.AddDoubleItem();

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(2, _items.Count());
        }

        [Fact]
        public void RequireDbContextHandler_NestedDoInDbContextWithException_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => _repoA.AddDoubleItemException());

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Empty(_items);
        }

        [Fact]
        public void RequireDbContextHandler_NormalQuery_ResultReceived()
        {
            Assert.Equal(0, _repoB.CountItems());
        }

        [Fact]
        public void RequireDbContextHandler_ModifyDbWithoutRequireDb_ExceptionThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _repoB.AddItemWithNoTransaction());

            Assert.Equal($"Detected dbcontext is changed by method AddItemWithNoTransaction at class {typeof(RepoB).FullName}, but transaction is not opened.", ex.Message);
        }

    }
}
