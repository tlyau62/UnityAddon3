﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.DAG
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

    [Trait("DependencyExceptions", "CircularDependency/DAG")]
    public class DAGTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveDAGDependency_NoExceptionThrown()
        {
            var container = new UnityContainer();
            new ApplicationContext(container, GetType().Namespace);
        }
    }
}