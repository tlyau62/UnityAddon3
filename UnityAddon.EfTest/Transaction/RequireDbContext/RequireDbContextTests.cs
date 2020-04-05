using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    public class RequireDbContextTests : EfDefaultTest
    {
        [Dependency]
        public IRepoA RepoA { get; set; }

        [Dependency]
        public IRepoB RepoB { get; set; }

        private DbSet<Item> _items => (DbContextFactory.IsOpen() ? DbContextFactory.Get() : DbContextFactory.Open()).Items;

        [Fact]
        public void RequireDbContextHandler_AddItem_ItemCreated()
        {
            RepoB.AddItem();

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal("testitem", _items.Single().Name);
        }

        [Fact]
        public void RequireDbContextHandler_ExceptionThrownInTransaction_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => RepoB.AddItemException());

            Assert.False(DbContextFactory.IsOpen());

            Assert.Empty(_items);
        }

        [Fact]
        public void RequireDbContextHandler_NestedCallOnRequireDbMethod_ItemCreated()
        {
            RepoA.AddDoubleItem();

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(2, _items.Count());
        }

        [Fact]
        public void RequireDbContextHandler_NestedDoInDbContextWithException_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => RepoA.AddDoubleItemException());

            Assert.False(DbContextFactory.IsOpen());

            Assert.Empty(_items);
        }

        [Fact]
        public void RequireDbContextHandler_NormalQuery_ResultReceived()
        {
            Assert.Equal(0, RepoB.CountItems());
        }

        [Fact]
        public void RequireDbContextHandler_ModifyDbWithoutRequireDb_ExceptionThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => RepoB.AddItemWithNoTransaction());

            Assert.Equal($"Detected dbcontext is changed by method AddItemWithNoTransaction at class {typeof(RepoB).FullName}, but transaction is not opened.", ex.Message);
        }

    }
}
