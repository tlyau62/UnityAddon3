﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.Config;
using UnityAddon.Ef;
using UnityAddon.EfTest.Common;
using Xunit;

namespace UnityAddon.EfTest.Transaction.Repository
{
    [ComponentScan(typeof(RepositoryTests))]
    [Import(typeof(UnityAddonEfConfig))]
    [Import(typeof(TestDbConfig))]
    public class RepositoryTests : EfTest<TestDbContext>
    {
        [Dependency]
        public IConfigs<AopInterceptorContainerOption> AopInterceptorContainerOption { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void Repository_Read()
        {
            var a = Sp.GetRequiredService<IRepo>();

            //Assert.Equal(0, Repo.CountItem());

            //Assert.False(DbContextFactory.IsOpen());
        }

        //[Fact]
        //public void Repository_WriteWithoutTransaction()
        //{
        //    var ex = Assert.Throws<InvalidOperationException>(() => Repo.InsertItem(new Item("testitem")));

        //    Assert.Equal($"Detected dbcontext is changed by method InsertItem at class {typeof(Repo).FullName}, but transaction is not opened.", ex.Message);
        //}
    }
}
