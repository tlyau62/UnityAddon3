using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.ScopeSingleton
{
    [Component]
    public class Service { }

    [ComponentScan(typeof(ScopeSingletonTests))]
    public class ScopeSingletonTests : UnityAddonTest
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
