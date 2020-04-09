using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.MultipleContext
{
    public interface IRepo
    {
        void InsertItem(Item item);
        void InsertItem2(Item2 item);
        int CountItem();
        int CountItem2();
    }

    [Repository]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<TestDbContext, Item>();

        private DbSet<Item2> _items2 => DbContextTemplate.GetEntity<TestDbContext2, Item2>();

        [RequireDbContext(Transactional = true)]
        [DataSource(typeof(TestDbContext))]
        public void InsertItem(Item item)
        {
            _items.Add(item);
        }

        [RequireDbContext(Transactional = true)]
        [DataSource(typeof(TestDbContext2))]
        public void InsertItem2(Item2 item)
        {
            _items2.Add(item);
        }

        [RequireDbContext(Transactional = false)]
        [DataSource(typeof(TestDbContext))]
        public int CountItem()
        {
            return _items.Count();
        }

        [RequireDbContext(Transactional = false)]
        [DataSource(typeof(TestDbContext2))]
        public int CountItem2()
        {
            return _items2.Count();
        }
    }
}
