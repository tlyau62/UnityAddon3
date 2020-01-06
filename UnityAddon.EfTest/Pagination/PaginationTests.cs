using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Ef;
using UnityAddon.Ef.Pagination;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Pagination
{
    [Trait("Pagination", "Pagination")]
    public class PaginationTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory _dbContextFactory;
        private IRepo _repo;
        private IDbContextTemplate<TestDbContext> _dbContextTemplate;

        public PaginationTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory>();
            _repo = _appContext.Resolve<IRepo>();
            _dbContextTemplate = _appContext.Resolve<IDbContextTemplate<TestDbContext>>();

            CreateDb();
        }

        public void Dispose()
        {
            DropDb();
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        public void Pagination_PaginationItems_CurrentPageReturned(int currentPage, int pageSize)
        {
            var dummyItems = new[] { "a", "b", "c", "d", "e" };

            foreach (var item in dummyItems)
            {
                _repo.InsertItem(new Item(item));
            }

            var page = _repo.GetItems(PageRequest.Of(currentPage, pageSize, Sort.Asc("name")));
            var content = page.Content.ToArray();
            var totalPage = (int)Math.Ceiling((double)dummyItems.Length / pageSize);

            Assert.Equal(page.Pagination.Page, currentPage);
            Assert.Equal(page.Pagination.Size, pageSize);
            Assert.Equal(page.Pagination.TotalPage, totalPage);

            for (var i = currentPage; i < totalPage; i++)
            {
                for (var j = 0; j < pageSize)

                Assert.Equal(dummyItems[i], content[i].Name);
            }
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
