using Microsoft.Extensions.Hosting;
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

    [Trait("DependencyExceptions", "CircularDependency/OptionalDependency")]
    public class OptionalDependencyTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IHost Host { get; set; }

        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveOptionalDependency_NoExceptionThrown()
        {
            Host.PreInstantiateSingleton();
        }
    }
}
