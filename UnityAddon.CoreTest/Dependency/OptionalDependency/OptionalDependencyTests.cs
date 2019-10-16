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

    [Component]
    public class Service
    {
        [OptionalDependency]
        public Helper Helper { get; set; }

        [OptionalDependency]
        public CustHelper CustHelper { get; set; }
    }

    [Trait("Dependency", "OptionalDependency")]
    public class OptionalDependencyTests
    {
        [Fact]
        public void PropertyFill_ResolveRegisteredOptionalDependency_BeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.NotNull(appContext.Resolve<Service>().Helper);
        }

        [Fact]
        public void PropertyFill_ResolveUnregisteredOptionalDependency_NullResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Null(appContext.Resolve<Service>().CustHelper);
        }
    }
}
