using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Scope.CoreContextResolve
{
    public class CoreContextResolveTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void CoreContextResolve()
        {
            IServiceProvider sp = null;

            using (var scope = Sp.CreateScope())
            {
                sp = scope.ServiceProvider.GetRequiredService<IServiceProvider>();
                // dispose scope should not dispose service provider
            }

            Assert.NotNull(sp.GetRequiredService<IServiceProvider>());
        }
    }
}
