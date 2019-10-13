using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using Xunit;

namespace UnityAddonTest.Dependency.CircularDependency.ComplexLoop
{
    [Component]
    class M1
    {
        public M1(M2 m2) { }
    }

    [Component]
    class M2
    {
        public M2(M3 m3, M4 m4, M5 m5) { }
    }

    [Component]
    class M3
    {
        public M3(M5 m5) { }
    }

    [Component]
    class M4
    {
        public M4(M5 m5) { }
    }

    [Component]
    class M5
    {
        public M5(M6 m6) { }
    }

    [Component]
    class M6
    {
        public M6(M7 m7) { }
    }

    [Component]
    class M7
    {
        public M7(M4 m4) { }
    }

    [Trait("Dependency", "CircularDependency/ComplexLoop")]
    public class ComplexLoopTests
    {
        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveComplexLoopDependency_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Throws<CircularDependencyException>(() => container.Resolve<M1>());
        }
    }
}
