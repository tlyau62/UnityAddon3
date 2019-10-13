using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using Xunit;

namespace UnityAddonTest.Dependency.CircularDependency.DAG2
{
    [Component]
    class G
    {
        public G(I i) { }
    }

    [Component]
    class H
    {
        public H(I i) { }
    }

    [Component]
    class I
    {
    }

    [Trait("Dependency", "CircularDependency/DAG2")]
    public class DAG2Tests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveDAG2Dependency_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            container.Resolve<G>();
        }
    }
}
