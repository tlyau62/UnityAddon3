using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Pagination;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.Pagination
{
    public interface IRepo
    {
        Page<Item> GetItems(Pageable pageable);
        void InsertItem(Item item);
    }

    [Component]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        [RequireDbContext(Transactional = false)]
        public Page<Item> GetItems(Pageable pageable)
        {
            return _items.ToPage(pageable);
        }

        [RequireDbContext(Transactional = true)]
        public void InsertItem(Item item)
        {
            _items.Add(item);
        }

    }
}
