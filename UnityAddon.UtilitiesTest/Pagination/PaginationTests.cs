using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Utilities.Pagination;
using Xunit;

namespace UnityAddon.EfTest.Pagination
{
    class Item
    {
        public Item(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }

    [Trait("Pagination", "Pagination")]
    public class PaginationTests
    {
        private IQueryable<Item> _queryResult;

        public PaginationTests()
        {
            var data = new List<Item>();

            data.Add(new Item("a", "z"));
            data.Add(new Item("b", "a"));
            data.Add(new Item("b", "r"));
            data.Add(new Item("d", "s"));
            data.Add(new Item("e", "m"));

            _queryResult = data.AsQueryable();
        }

        [Fact]
        public void Pagination_PaginationSortingOnItems_ItemsOrdered()
        {
            var pageable = PageRequest.Of(0, _queryResult.Count(), Sort.Desc("Id").ThenAsc("Name"));
            var page = _queryResult.ToPage(pageable);

            Assert.Equal(page.Content.Select(i => i.Id), new string[] { "e", "d", "b", "b", "a" });
            Assert.Equal(page.Content.Select(i => i.Name), new string[] { "m", "s", "a", "r", "z" });
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(2, 4)]
        public void Pagination_PaginationSlicingOnItems_ItemsSliced(int pageNo, int size)
        {
            var pageable = PageRequest.Of(pageNo, size);
            var page = _queryResult.ToPage(pageable);
            var expected = _queryResult.Skip(pageNo * size).Take(size);

            Assert.Equal(page.Content.Select(i => i.Id), expected.Select(i => i.Id));
            Assert.Equal(page.Content.Select(i => i.Name), expected.Select(i => i.Name));
        }

    }
}
