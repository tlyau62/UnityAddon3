using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Cache;
using UnityAddon.Core;
using UnityAddon.Core.Context;
using Xunit;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CacheTest
{
    [ComponentScan(typeof(CacheTest))]
    [Import(typeof(UnityAddonCacheConfig))]
    public class CacheTest : UnityAddonTest
    {
        [Dependency]
        public IMyRepository myRepository { get; set; }

        [Dependency]
        public IMyService myService { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void Test()
        {
            // cache miss
            {
                var en = myRepository.GetByID(1);
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(1, MyRepository.GetByIDCount);
            }

            // cache miss
            {
                var en = myRepository.GetByID(2);
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(2, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByID(1);
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(2, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByID(2);
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(2, MyRepository.GetByIDCount);
            }

            // cache miss
            {
                var en = myRepository.GetByFullName("n", "x");
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(1, MyRepository.GetByFullNameCount);
            }

            // cache miss
            {
                var en = myRepository.GetByFullName("m", "s");
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(2, MyRepository.GetByFullNameCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("n", "x");
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(2, MyRepository.GetByFullNameCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("m", "s");
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(2, MyRepository.GetByFullNameCount);
            }

            // invalidate
            myService.Update();

            // cache miss
            {
                var en = myRepository.GetByID(1);
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(3, MyRepository.GetByIDCount);
            }

            // cache miss
            {
                var en = myRepository.GetByID(2);
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByID(1);
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(4, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByID(2);
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByIDCount);
            }

            // cache miss
            {
                var en = myRepository.GetByFullName("n", "x");
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(3, MyRepository.GetByFullNameCount);
            }

            // cache miss
            {
                var en = myRepository.GetByFullName("m", "s");
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByFullNameCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("n", "x");
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(4, MyRepository.GetByFullNameCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("m", "s");
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByFullNameCount);
            }

            // doesn't invalidate cache of MyRepository
            myService.DoSomething();

            // cache hit
            {
                var en = myRepository.GetByID(1);
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(4, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByID(2);
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByIDCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("n", "x");
                Assert.Equal(1, en.ID);
                Assert.Equal("n", en.Namespace);
                Assert.Equal("x", en.Name);
                Assert.Equal(4, MyRepository.GetByFullNameCount);
            }

            // cache hit
            {
                var en = myRepository.GetByFullName("m", "s");
                Assert.Equal(2, en.ID);
                Assert.Equal("m", en.Namespace);
                Assert.Equal("s", en.Name);
                Assert.Equal(4, MyRepository.GetByFullNameCount);
            }
        }
    }
}
