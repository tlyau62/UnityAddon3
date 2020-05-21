using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.EfTest.Transaction.Repository
{
    [ComponentScan]
    [ContextConfiguration(typeof(UnityAddonEfConfig), typeof(TestDbConfig<TestDbContext>))]
    public class RepositoryTests : UnityAddonEfTest
    {
        public RepositoryTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IRepo Repo { get; set; }

        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Fact]
        public void Repository_Read()
        {
            Assert.Equal(0, Repo.CountItem());

            Assert.False(DbContextFactory.IsOpen());
        }

        [Fact]
        public void Repository_WriteWithoutTransaction()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Repo.InsertItem(new Item("testitem")));

            Assert.Equal($"Detected dbcontext is changed by method InsertItem at class {typeof(Repo).FullName}, but transaction is not opened.", ex.Message);
        }
    }
}
