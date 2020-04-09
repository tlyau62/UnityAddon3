using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    public class RepositoryTests : EfDefaultTest<TestDbContext>
    {
        [Dependency]
        public IRepo Repo { get; set; }

        [Fact]
        public void RequireDbContextHandler_QueryItem_ResultReceived()
        {
            Assert.Equal(0, Repo.CountItem());

            Assert.False(DbContextFactory.IsOpen());
        }

        [Fact]
        public void RequireDbContextHandler_ModifyDbWithoutTransaction_ExceptionThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Repo.InsertItem(new Item("testitem")));

            Assert.Equal($"Detected dbcontext is changed by method InsertItem at class {typeof(Repo).FullName}, but transaction is not opened.", ex.Message);
        }

    }
}
