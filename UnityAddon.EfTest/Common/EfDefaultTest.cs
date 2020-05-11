using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Ef;

namespace UnityAddon.EfTest.Common
{
    public class EfDefaultTest<TDbContext> : IDisposable where TDbContext : DbContext
    {
        [Dependency]
        public IDbContextFactory<TDbContext> DbContextFactory { get; set; }

        public EfDefaultTest()
        {
            ((IUnityAddonSP)new HostBuilder()
                .RegisterUA()
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(GetType().Assembly, GetType().Namespace, "UnityAddon.EfTest.Common"));
                })
                .EnableUnityAddonEf()
                .Build()
                .Services)
                .BuildUp(GetType(), this);

            DbSetupUtility.CreateDb(DbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
        }
    }
}
