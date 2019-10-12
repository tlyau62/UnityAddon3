using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using Xunit;

namespace UnityAddonTest.Dependency.Dependency
{
    public class CustHelper
    {
    }

    [Component]
    public class Service
    {
        [Dependency]
        public CustHelper CustHelper { get; set; }
    }

    [Trait("Dependency", "Dependency")]
    public class DependencyTests
    {
        [Fact]
        public void PropertyFill_ResolveUnregisteredDependency_NoSuchBeanDefinitionExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Throws<NoSuchBeanDefinitionException>(() => container.Resolve<Service>());
        }
    }
}
