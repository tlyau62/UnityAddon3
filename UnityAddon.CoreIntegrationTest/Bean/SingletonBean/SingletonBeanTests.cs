using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.SingletonBean
{
    [Component]
    [Scope(ScopeType.Singleton)]
    public class SingletonService
    {
    }

    [ComponentScan]
    public class SingletonBeanTests : UnityAddonTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void SingletonBean()
        {
            Assert.Same(Sp.GetRequiredService<SingletonService>(), Sp.GetRequiredService<SingletonService>());
        }
    }
}
