using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.OptionalDependency
{
    [Component]
    class D
    {
        [Dependency]
        public A A { get; set; }

        [OptionalDependency]
        public B B { get; set; }
    }

    [Component]
    class A
    {
        [OptionalDependency]
        public B B { get; set; }

        [OptionalDependency]
        public C C { get; set; }
    }

    class B
    {
    }

    class C
    {
    }

    public class OptionalDependencyTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void OptionalDependency()
        {
            var types = new[] { typeof(A), typeof(D) };

            foreach (var type in types)
            {
                Sp.GetRequiredService(type);
            }
        }
    }
}
