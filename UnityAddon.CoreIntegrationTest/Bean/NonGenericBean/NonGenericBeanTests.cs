using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.Bean.NonGenericBean
{
    public interface IB { IC C { get; set; } }
    public interface IC { }

    public abstract class AbstractA
    {
        [Dependency]
        public IB B { get; set; }
    }

    [Component]
    public class A : AbstractA
    {
    }

    [Component]
    public class B : IB
    {
        [Dependency]
        public IC C { get; set; }
    }

    [Component]
    public class C : IC
    {
    }

    [ComponentScan]
    public class NonGenericBeanTests : UnityAddonTest
    {
        [Dependency]
        public AbstractA A { get; set; }

        [Dependency]
        public IB B { get; set; }

        [Dependency]
        public IC C { get; set; }

        [Fact]
        public void NonGenericBean()
        {
            Assert.Same(A.B, B);
            Assert.Same(B.C, C);
        }
    }
}
