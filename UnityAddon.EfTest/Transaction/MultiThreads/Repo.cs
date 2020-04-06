using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.Transaction.MultiThreads
{
    public interface IRepo
    {
        int CountItem();
        void InsertItem(Item item);
        void InsertItemWithException(Item item);
    }

    [Repository]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<TestDbContext, Item>();

        [RequireDbContext(Transactional = true)]
        public void InsertItem(Item item)
        {
            _items.Add(item);
        }

        public int CountItem()
        {
            return _items.Count();
        }

        [RequireDbContext(Transactional = true)]
        public void InsertItemWithException(Item item)
        {
            InsertItem(item);

            throw new Exception();
        }

    }
}
