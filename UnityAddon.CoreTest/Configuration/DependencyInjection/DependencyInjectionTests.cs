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

    public class Service
    {
        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        public CtorHelper CtorHelper { get; set; }

        public Service(CtorHelper ctorHelper)
        {
            CtorHelper = ctorHelper;
        }
    }

    [Configuration]
    public class Config
    {
        [Bean]
        public virtual Service CreateService(CtorHelper ctorHelper)
        {
            return new Service(ctorHelper);
        }
    }

    [Trait("Configuration", "DependencyInjection")]
    public class DependencyInjectionTests : UnityAddonDefaultTest
    {
        [Dependency]
        public Service Service { get; set; }

        [Dependency]
        public CtorHelper CtorHelper { get; set; }

        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        [Fact]
        public void BuildStrategy_BeanMethodDependency_BeanInjected()
        {
            Assert.Same(CtorHelper, Service.CtorHelper);
            Assert.Same(PropertyHelper, Service.PropertyHelper);
        }
    }
}
