using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.RequireDbContext
{
    [ComponentScan(typeof(RequireDbContextTests))]
    [Import(typeof(UnityAddonEfConfig))]
    [Import(typeof(TestDbConfig<TestDbContext>))]
    public class RequireDbContextTests : UnityAddonEfTest
    {
        [Dependency]
        public IRepoA RepoA { get; set; }

        [Dependency]
        public IRepoB RepoB { get; set; }

        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        private DbSet<Item> Items => DbContextTemplate.GetEntity<TestDbContext, Item>();

        [Fact]
        public void RequireDbContextHandler_AddItem_ItemCreated()
        {
            RepoB.AddItem();

            Assert.False(DbContextFactory.IsOpen());

            DbContextTemplate.ExecuteQuery<TestDbContext>(ctx => Assert.Equal("testitem", Items.Single().Name));

            Assert.False(DbContextFactory.IsOpen());
        }

        [Fact]
        public void RequireDbContextHandler_ExceptionThrownInTransaction_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => RepoB.AddItemException());

            Assert.False(DbContextFactory.IsOpen());

            DbContextTemplate.ExecuteQuery<TestDbContext>(ctx => Assert.Empty(Items));

            Assert.False(DbContextFactory.IsOpen());
        }

        [Fact]
        public void RequireDbContextHandler_NestedCallOnRequireDbMethod_ItemCreated()
        {
            RepoA.AddDoubleItem();

            Assert.False(DbContextFactory.IsOpen());

            DbContextTemplate.ExecuteQuery<TestDbContext>(ctx => Assert.Equal(2, Items.Count()));
        }

        [Fact]
        public void RequireDbContextHandler_NestedDoInDbContextWithException_DbRollback()
        {
            Assert.Throws<InvalidOperationException>(() => RepoA.AddDoubleItemException());

            Assert.False(DbContextFactory.IsOpen());

            DbContextTemplate.ExecuteQuery<TestDbContext>(ctx => Assert.Empty(Items));
        }

        [Fact]
        public void RequireDbContextHandler_NormalQuery_ResultReceived()
        {
            DbContextTemplate.ExecuteQuery<TestDbContext>(ctx => Assert.Equal(0, RepoB.CountItems()));
        }

        [Fact]
        public void RequireDbContextHandler_ModifyDbWithoutRequireDb_ExceptionThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => RepoB.AddItemWithNoTransaction());

            Assert.Equal($"Detected dbcontext is changed by method AddItemWithNoTransaction at class {typeof(RepoB).FullName}, but transaction is not opened.", ex.Message);
        }

    }
}
