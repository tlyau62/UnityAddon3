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
    public class EfTest<TDbContext> : UnityAddonTest, IDisposable where TDbContext : DbContext
    {
        [Dependency]
        public IDbContextFactory<TDbContext> DbContextFactory { get; set; }

        public EfTest()
        {
            DbSetupUtility.CreateDb(DbContextFactory);
        }

        public void Dispose()
        {
            DbSetupUtility.DropDb(DbContextFactory);
        }
    }
}
