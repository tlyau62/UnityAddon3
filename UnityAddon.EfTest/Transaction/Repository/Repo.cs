using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.Transaction.Repository
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
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        public void InsertItem(Item item)
        {
            _items.Add(item);
        }

        public int CountItem()
        {
            return _items.Count();
        }

        public void InsertItemWithException(Item item)
        {
            InsertItem(item);

            throw new Exception();
        }

    }
}
