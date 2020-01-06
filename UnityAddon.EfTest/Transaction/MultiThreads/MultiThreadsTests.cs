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
    public class MultiThreadsTests : IDisposable
    {
        private ApplicationContext _appContext;
        private IDbContextFactory<TestDbContext> _dbContextFactory;
        private IRepo _repo;
        private Random _random = new Random();

        public MultiThreadsTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace, typeof(TestDbContext).Namespace);
            _dbContextFactory = _appContext.Resolve<IDbContextFactory<TestDbContext>>();
            _repo = _appContext.Resolve<IRepo>();

            DbSetupUtility.CreateDb(_dbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(_dbContextFactory);
        }

        [Theory]
        [InlineData(100, 20)]
        [InlineData(500, 100)]
        public void RequireDbContextHandler_MultiThreads_ItemsInserted(int itemsAccepted, int itemsRejected)
        {
            var tasks = new List<Task>();

            Action<object> action = (index) =>
            {
                Thread.Sleep((int)(_random.NextDouble() * 100)); // add some random delay

                _repo.InsertItem(new Item(Guid.NewGuid().ToString()));
            };

            Action<object> exceptionAction = (index) =>
            {
                Thread.Sleep((int)(_random.NextDouble() * 100)); // add some random delay

                try
                {
                    _repo.InsertItemWithException(new Item(Guid.NewGuid().ToString()));
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

            Assert.False(_dbContextFactory.IsOpen());

            Assert.Equal(itemsAccepted, _repo.CountItem());
        }

    }
}
