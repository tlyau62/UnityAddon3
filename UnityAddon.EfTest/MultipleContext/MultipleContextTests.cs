using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Context;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using UnityAddon.Core.Util.ComponentScanning;
using Xunit;

namespace UnityAddon.EfTest.MultipleContext
{
    [Trait("Transaction", "Repository")]
    public class MultipleContextTests : IDisposable
    {
        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        [Dependency]
        public IDbContextFactory<TestDbContext2> DbContextFactory2 { get; set; }

        [Dependency]
        public IRepo Repo { get; set; }

        public MultipleContextTests()
        {
            var host = new HostBuilder()
                .RegisterUA()
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(GetType().Assembly, GetType().Namespace, "UnityAddon.EfTest.Common"));
                })
                .EnableUnityAddonEf()
                .Build();

            ((IUnityAddonSP)host.Services).BuildUp(this);

            DbSetupUtility.CreateDb(DbContextFactory);
            DbSetupUtility.CreateDb(DbContextFactory2);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
            DbSetupUtility.DropDb(DbContextFactory2);
        }

        [Fact]
        public void RepositoryInterceptor_InsertItemsOnMultipleDbContext_ItemsInserted()
        {
            var item = new Item("testitem");
            var item2a = new Item2("testitem2a");
            var item2b = new Item2("testitem2b");

            Repo.InsertItem(item);
            Repo.InsertItem2(item2a);
            Repo.InsertItem2(item2b);

            Assert.Equal(1, Repo.CountItem());
            Assert.Equal(2, Repo.CountItem2());

            Assert.False(DbContextFactory.IsOpen());
            Assert.False(DbContextFactory2.IsOpen());
        }

    }
}
