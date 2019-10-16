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

    [Trait("Bean", "NonGenericBean")]
    public class NonGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnNonGenericBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var a = appContext.Resolve<AbstractA>();
            var b = appContext.Resolve<IB>();
            var c = appContext.Resolve<IC>();

            Assert.Same(a.B, b);
            Assert.Same(b.C, c);
        }
    }
}
