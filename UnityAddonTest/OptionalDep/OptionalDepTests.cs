using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.OptionalDep
{
    [Component]
    public class Helper
    {
    }

    public class CustHelper
    {
    }

    [Component]
    public class Service
    {
        [OptionalDependency]
        public Helper Helper { get; set; }

        [OptionalDependency]
        public CustHelper CustHelper { get; set; }
    }

    [Trait("Dependency", "OptionalDependency")]
    public class OptionalDepTests
    {
        [Fact]
        public void PropertyFill_ResolveRegisteredOptionalDependency_BeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.NotNull(container.Resolve<Service>().Helper);
        }

        [Fact]
        public void PropertyFill_ResolveUnregisteredOptionalDependency_NullResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Null(container.Resolve<Service>().CustHelper);
        }
    }
}
