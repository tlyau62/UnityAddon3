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

namespace UnityAddon.CoreIntegrationTest.Scope
{
    [Component]
    public class Service { }

    public class ScopeTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

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
