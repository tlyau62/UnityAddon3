using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.MultiThreads
{
    [Trait("Transaction", "MultiThreads")]
    public class MultiThreadsTests : EfDefaultTest<TestDbContext>
    {
        [Dependency]
        public IRepo Repo { get; set; }

        private readonly Random _random = new Random();

        [Theory]
        [InlineData(100, 20)]
        [InlineData(500, 100)]
        public void RequireDbContextHandler_MultiThreads_ItemsInserted(int itemsAccepted, int itemsRejected)
        {
            var tasks = new List<Task>();

            Action<object> action = (index) =>
            {
                Thread.Sleep((int)(_random.NextDouble() * 100)); // add some random delay

                Repo.InsertItem(new Item(Guid.NewGuid().ToString()));
            };

            Action<object> exceptionAction = (index) =>
            {
                Thread.Sleep((int)(_random.NextDouble() * 100)); // add some random delay

                try
                {
                    Repo.InsertItemWithException(new Item(Guid.NewGuid().ToString()));
                }
                catch (Exception)
                {
                }
            };

            for (int i = 0; i < itemsAccepted; i++)
            {
                tasks.Add(Task.Factory.StartNew(action, i));
            }

            for (int i = 0; i < itemsRejected; i++)
            {
                tasks.Add(Task.Factory.StartNew(exceptionAction, i));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.False(DbContextFactory.IsOpen());

            Assert.Equal(itemsAccepted, Repo.CountItem());
        }

    }
}
