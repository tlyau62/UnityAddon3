using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.MultipleContext
{
    [Trait("Transaction", "Repository")]
    public class MultipleContextTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory<TestDbContext> _dbContextFactory;
        private IDbContextFactory<TestDbContext2> _dbContextFactory2;
        private IRepo _repo;

        public MultipleContextTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory<TestDbContext>>();
            _dbContextFactory2 = _appContext.Resolve<IDbContextFactory<TestDbContext2>>();
            _repo = _appContext.Resolve<IRepo>();

            DbSetupUtility.CreateDb(_dbContextFactory);
            DbSetupUtility.CreateDb(_dbContextFactory2);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(_dbContextFactory);
            DbSetupUtility.DropDb(_dbContextFactory2);
        }

        [Fact]
        public void RepositoryInterceptor_InsertItemsOnMultipleDbContext_ItemsInserted()
        {
            var item = new Item("testitem");
            var item2 = new Item2("testitem2");

            _repo.InsertItem(item);
            _repo.InsertItem2(item2);

            Assert.Equal(1, _repo.CountItem());
            Assert.Equal(1, _repo.CountItem2());

            Assert.False(_dbContextFactory.IsOpen());
            Assert.False(_dbContextFactory2.IsOpen());
        }

    }
}
