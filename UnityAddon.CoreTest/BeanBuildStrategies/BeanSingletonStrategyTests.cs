using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.BeanBuildStrategies
{
    [Component]
    public class Service { }

    public class BeanSingletonStrategyTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void CreateScopeWithSingletonBean()
        {
            using (var scope = Sp.CreateScope())
            {
                scope.ServiceProvider.GetService<ILogger<Service>>();
            }

            Assert.NotNull(Sp.GetService<ILogger<Service>>());
        }
    }
}
