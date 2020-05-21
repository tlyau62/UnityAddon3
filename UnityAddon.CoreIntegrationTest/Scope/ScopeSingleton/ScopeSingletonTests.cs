using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.ScopeSingleton
{
    [Component]
    public class Service { }

    [ComponentScan]
    public class ScopeSingletonTests : UnityAddonTest
    {
        public ScopeSingletonTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

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
