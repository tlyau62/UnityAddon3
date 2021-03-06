﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Test;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Scope.CoreContextResolve
{
    public class CoreContextResolveTests : UnityAddonTest
    {
        public CoreContextResolveTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void CoreContextResolve()
        {
            IUnityAddonSP sp = null;

            using (var scope = Sp.CreateScope())
            {
                sp = scope.ServiceProvider.GetRequiredService<IUnityAddonSP>();
                // dispose scope should not dispose service provider
            }

            Assert.NotNull(sp.GetRequiredService<IUnityAddonSP>());
        }
    }
}
