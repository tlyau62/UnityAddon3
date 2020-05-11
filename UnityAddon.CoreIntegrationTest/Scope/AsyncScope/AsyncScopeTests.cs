using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Scope.AsyncScope
{
    [Component]
    public class Counter
    {
        private readonly object _lockObj = new object();

        public int Count { get; private set; } = 0;

        public void Inc()
        {
            lock (_lockObj)
            {
                Count++;
            }
        }
    }

    [Component]
    public class SingletonService : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Component]
    [Scope(ScopeType.Scoped)]
    public class Service : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        [Dependency]
        public Counter Counter { get; set; }

        [Dependency]
        public SingletonService SingletonService { get; set; }

        public void Do()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("something wrong");
            }

            Counter.Inc();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Component]
    [Scope(ScopeType.Scoped)]
    public class Request : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        [Dependency]
        public Service Service { get; set; }

        [Dependency]
        public SingletonService SingletonService { get; set; }

        public void Start()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("something wrong");
            }
            Service.Do();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public class AsyncScopeTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        private ConcurrentDictionary<Service, int> _serviceDict = new ConcurrentDictionary<Service, int>();

        private ConcurrentDictionary<Request, int> _reqDict = new ConcurrentDictionary<Request, int>();

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void AsyncScope(int size)
        {
            var tasks = new Task[size];

            var requestsSet = new HashSet<Request>();
            var serviceSet = new HashSet<Service>();

            for (var i = 0; i < size; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => CreateRequest());
            }

            Task.WaitAll(tasks);

            Assert.Equal(size, Sp.GetRequiredService<Counter>().Count);
            Assert.Equal(size, _serviceDict.Keys.Count);
            Assert.Equal(size, _reqDict.Keys.Count);
        }

        private void CreateRequest()
        {
            Request req = null;
            var globalReq = Sp.GetRequiredService<Request>();
            var singletonService = Sp.GetRequiredService<SingletonService>();

            using (var scope = Sp.CreateScope())
            {
                req = scope.ServiceProvider.GetRequiredService<Request>();
                req.Start();

                Assert.Same(req, scope.ServiceProvider.GetRequiredService<Request>());
                Assert.Same(req.Service, scope.ServiceProvider.GetRequiredService<Request>().Service);
                Assert.Same(singletonService, Sp.GetRequiredService<SingletonService>());
            }

            Assert.True(req.IsDisposed);
            Assert.True(req.Service.IsDisposed);
            Assert.False(globalReq.IsDisposed);
            Assert.False(globalReq.Service.IsDisposed);
            Assert.Same(singletonService, Sp.GetRequiredService<SingletonService>());
            Assert.False(Sp.GetRequiredService<SingletonService>().IsDisposed);
            Assert.Same(globalReq, Sp.GetRequiredService<Request>());
            Assert.Same(globalReq.Service, Sp.GetRequiredService<Request>().Service);

            _serviceDict[req.Service] = 1;
            _reqDict[req] = 1;
        }
    }
}
