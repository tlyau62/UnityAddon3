using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.DependencyInjection
{
    [Component]
    public class CtorHelper { }

    [Component]
    public class PropertyHelper { }

    [Configuration]
    public class Config
    {
        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        public CtorHelper CtorHelper { get; set; }

        [Bean]
        public virtual int CreateService(CtorHelper ctorHelper)
        {
            CtorHelper = ctorHelper;

            return 10;
        }
    }

    [ComponentScan(typeof(DependencyInjectionTests))]
    public class DependencyInjectionTests : UnityAddonTest
    {
        [Dependency]
        public CtorHelper CtorHelper { get; set; }

        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        [Dependency]
        public Config Config { get; set; }

        [Dependency]
        public int TriggerConfigBean { get; set; }

        [Fact]
        public void DependencyInjection()
        {
            Assert.Same(CtorHelper, Config.CtorHelper);
            Assert.Same(PropertyHelper, Config.PropertyHelper);
        }
    }
}
