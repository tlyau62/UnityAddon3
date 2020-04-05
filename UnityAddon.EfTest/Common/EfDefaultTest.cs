using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Ef;

namespace UnityAddon.EfTest.Common
{
    public class EfDefaultTest : IDisposable
    {
        [Dependency]
        public IDbContextFactory<TestDbContext> DbContextFactory { get; set; }

        public EfDefaultTest()
        {
            new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace, "UnityAddon.EfTest.Common")
                .EnableUnityAddonEf()
                .BuildUA()
                .BuildTestUA(this);

            DbSetupUtility.CreateDb(DbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
        }
    }
}
