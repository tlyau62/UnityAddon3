using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using Xunit;

namespace UnityAddonTest.Dependency.CircularDependency.DAG
{
    [Component]
    class D
    {
        public D(E e) { }
    }

    [Component]
    class E
    {
        public E(F f) { }
    }

    [Component]
    class F
    {
    }

    [Trait("Dependency", "CircularDependency/DAG")]
    public class DAGTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveDAGDependency_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            container.Resolve<D>();
        }
    }
}
