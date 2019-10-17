using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.Transaction.RequireDbContext
{
    public interface IRepoA
    {
        void AddDoubleItem();
        void AddDoubleItemException();
    }

    public interface IRepoB
    {
        void AddItem();
        void AddItemException();
        int CountItems();
        void AddItemWithNoTransaction();
    }

    [Component]
    public class RepoA : IRepoA
    {
        [Dependency]
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        [Dependency]
        public IRepoB RepoB { get; set; }

        [RequireDbContext(Transactional = true)]
        public void AddDoubleItem()
        {
            _items.Add(new Item("testitem"));
            RepoB.AddItem();
        }

        [RequireDbContext(Transactional = true)]
        public void AddDoubleItemException()
        {
            _items.Add(new Item("testitem"));
            RepoB.AddItemException();
        }
    }

    [Component]
    public class RepoB : IRepoB
    {
        [Dependency]
        public IDbContextTemplate<TestDbContext> DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<Item>();

        [RequireDbContext(Transactional = true)]
        public void AddItem()
        {
            _items.Add(new Item("testitem"));
        }

        [RequireDbContext(Transactional = true)]
        public void AddItemException()
        {
            _items.Add(new Item("testitem"));

            throw new InvalidOperationException();
        }

        [RequireDbContext]
        public int CountItems()
        {
            return _items.Count();
        }

        [RequireDbContext]
        public void AddItemWithNoTransaction()
        {
            _items.Add(new Item("testitem"));
        }
    }
}
