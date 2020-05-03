using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
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

    public class ComplexDAGTests : DefaultTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void ComplexDAG()
        {
            var types = new[] { typeof(N1), typeof(N2), typeof(N3), typeof(N4), typeof(N5), typeof(N6), typeof(N7) };

            foreach (var type in types)
            {
                Sp.GetRequiredService(type);
            }
        }
    }
}
