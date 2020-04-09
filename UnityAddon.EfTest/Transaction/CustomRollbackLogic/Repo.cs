using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Ef.Transaction;
using UnityAddon.EfTest.Common;

namespace UnityAddon.EfTest.Transaction.CustomRollbackLogic
{
    public interface IRepo
    {
        GenericResult<string> AddItemGeneric(bool isSuccess);
        Result AddItem(bool isSuccess);
        ConcreteGenericResult<string> AddStringItemConcreteGeneric(bool isSuccess);
        ConcreteGenericResult<int> AddIntItemConcreteGeneric(bool isSuccess);
    }

    [Component]
    public class Repo : IRepo
    {
        [Dependency]
        public IDbContextTemplate DbContextTemplate { get; set; }

        private DbSet<Item> _items => DbContextTemplate.GetEntity<TestDbContext, Item>();

        [RequireDbContext(Transactional = true)]
        public GenericResult<string> AddItemGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new GenericResult<string>()
            {
                IsSuccess = isSuccess
            };
        }

        [RequireDbContext(Transactional = true)]
        public Result AddItem(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new TestResult()
            {
                IsSuccess = isSuccess
            };
        }

        [RequireDbContext(Transactional = true)]
        public ConcreteGenericResult<string> AddStringItemConcreteGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new ConcreteGenericResult<string>()
            {
                IsSuccess = isSuccess
            };
        }

        /// <summary>
        /// will never rollback
        /// </summary>
        [RequireDbContext(Transactional = true)]
        public ConcreteGenericResult<int> AddIntItemConcreteGeneric(bool isSuccess)
        {
            _items.Add(new Item("testitem"));

            return new ConcreteGenericResult<int>()
            {
                IsSuccess = isSuccess
            };
        }
    }
}
