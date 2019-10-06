using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Dependency.Bean.NonGenericBean
{
    public interface IB { IC c { get; set; } }
    public interface IC { }

    public abstract class AbstractA
    {
        [Dependency]
        public IB b { get; set; }
    }

    [Component]
    public class A : AbstractA
    {
    }

    [Component]
    public class B : IB
    {
        [Dependency]
        public IC c { get; set; }
    }

    [Component]
    public class C : IC
    {
    }

    [Trait("Bean", "NonGenericBean")]
    public class NonGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Same(container.Resolve<IB>(), container.Resolve<A>().b);
            Assert.Same(container.Resolve<IC>(), container.Resolve<IB>().c);
        }
    }
}
