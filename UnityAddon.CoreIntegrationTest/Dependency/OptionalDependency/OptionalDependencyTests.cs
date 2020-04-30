using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.OptionalDependency
{
    [Component]
    public class Helper
    {
    }

    public class CustHelper
    {
    }

    public interface ITest { }

    [Component]
    public class Service
    {
        [OptionalDependency]
        public Helper Helper { get; set; }

        [OptionalDependency]
        public CustHelper CustHelper { get; set; }

        [OptionalDependency]
        public ITest Test { get; set; }
    }

    [Trait("Dependency", "OptionalDependency")]
    public class OptionalDependencyTests : UnityAddonDefaultTest
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void PropertyFill_ResolveRegisteredOptionalDependency_BeanResolved()
        {
            Assert.NotNull(Service.Helper);
        }

        [Fact]
        public void PropertyFill_ResolveUnregisteredOptionalDependency_NullResolved()
        {
            Assert.Null(Service.CustHelper);
        }

        [Fact]
        public void PropertyFill_ResolveUnregisteredOptionalInterfaceDependency_NullResolved()
        {
            Assert.Null(Service.Test);
        }
    }
}
