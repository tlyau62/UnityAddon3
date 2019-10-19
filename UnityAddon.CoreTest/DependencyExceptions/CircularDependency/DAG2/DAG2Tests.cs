using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.DAG2
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

    [Trait("DependencyExceptions", "CircularDependency/DAG2")]
    public class DAG2Tests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveDAG2Dependency_NoExceptionThrown()
        {
            var container = new UnityContainer();
            new ApplicationContext(container, GetType().Namespace);
        }
    }
}
