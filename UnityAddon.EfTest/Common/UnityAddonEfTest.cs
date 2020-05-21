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
using UnityAddon.Test;

namespace UnityAddon.EfTest.Common
{
    public class UnityAddonEfTest : UnityAddonTest, IDisposable
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public DataSourceExtractor DataSourceExtractor { get; set; }

        public UnityAddonEfTest(UnityAddonTestFixture testFixture) : this(testFixture, false)
        {
        }

        public UnityAddonEfTest(UnityAddonTestFixture testFixture, bool isDefered) : base(testFixture, isDefered)
        {
            foreach (var datasource in DataSourceExtractor.DataSources)
            {
                DbSetupUtility.CreateDb((dynamic)Sp.GetRequiredService(typeof(IDbContextFactory<>).MakeGenericType(datasource)));
            }
        }

        public void Dispose()
        {
            foreach (var datasource in DataSourceExtractor.DataSources)
            {
                DbSetupUtility.DropDb((dynamic)Sp.GetRequiredService(typeof(IDbContextFactory<>).MakeGenericType(datasource)));
            }
        }
    }
}
