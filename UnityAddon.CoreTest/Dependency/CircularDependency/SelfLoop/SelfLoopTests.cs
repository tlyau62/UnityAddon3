using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.CircularDependency.SelfLoop
{
    [Component]
    class A
    {
        public A(A a) { }
    }

    [Trait("Dependency", "CircularDependency/SelfLoop")]
    public class SelfLoopTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveSelfLoopDependency_ExceptionThrown()
        {
            var container = new UnityContainer();

            Assert.Throws<CircularDependencyException>(() => new ApplicationContext(container, GetType().Namespace));
        }
    }
}
