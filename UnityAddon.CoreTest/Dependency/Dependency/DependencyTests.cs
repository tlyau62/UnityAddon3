using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.Dependency
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

            Assert.Throws<NoSuchBeanDefinitionException>(() => appContext.Resolve<Service>());
        }
    }
}
