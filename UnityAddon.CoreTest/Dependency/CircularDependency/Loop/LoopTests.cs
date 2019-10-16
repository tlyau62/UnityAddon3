using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.CircularDependency.Loop
{
    public interface IA { }
    public interface IB { }

    [Component]
    class A : IA
    {
        public A(IB b) { }
    }

    [Component]
    class B : IB
    {
        public B(IA a) { }
    }

    [Trait("Dependency", "CircularDependency/Loop")]
    public class SelfLoopTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveLoopDependency_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Throws<CircularDependencyException>(() => container.Resolve<A>());
        }
    }
}
