using Microsoft.Extensions.Hosting;
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
    public class DAG2Tests : UnityAddonDefaultTest
    {
        [Dependency]
        public IHost Host { get; set; }

        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveDAG2Dependency_NoExceptionThrown()
        {
            Host.PreInstantiateSingleton();
        }
    }
}
