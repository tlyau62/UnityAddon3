﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.ComplexDAG
{
    [Component]
    class N1
    {
        public N1(N2 n2) { }
    }

    [Component]
    class N2
    {
        public N2(N3 n3, N4 n4, N5 n5) { }
    }

    [Component]
    class N3
    {
        public N3(N5 n5) { }
    }

    [Component]
    class N4
    {
        public N4(N5 n5) { }
    }

    [Component]
    class N5
    {
        public N5(N6 n6) { }
    }

    [Component]
    class N6
    {
        public N6() { }
    }

    [Component]
    class N7
    {
        public N7(N4 n4) { }
    }

    [Trait("DependencyExceptions", "CircularDependency/ComplexDAG")]
    public class ComplexDAGTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveComplexDAGDependency_NoExceptionThrown()
        {
            var container = new UnityContainer();
            
            new ApplicationContext(container, GetType().Namespace);
        }
    }
}