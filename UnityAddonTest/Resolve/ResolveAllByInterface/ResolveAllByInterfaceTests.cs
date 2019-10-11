using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;
using Unity.Lifetime;

namespace UnityAddonTest.Resolve.ResolveAllByInterface
{
    public interface IService { }

    [Component]
    [Qualifier("a")]
    public class A : IService { }

    [Component]
    [Qualifier("b")]
    public class B : IService { }

    public class C : IService { }

    [Trait("Resolve", "ResolveAllByInterface")]
    public class ResolveAllByInterfaceTests
    {
        [Fact]
        public void ApplicationContext_ResolveAllByInterface_AllQualifiedBeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            appContext.RegisterType<IService, C>("c", new SingletonLifetimeManager());

            var a = appContext.Resolve<IService>("a");
            var b = appContext.Resolve<IService>("b");
            var c = appContext.Resolve<IService>("c");

            var resolveAll = appContext.ResolveAll<IService>();

            Assert.Same(a, resolveAll[0]);
            Assert.Same(b, resolveAll[1]);
            Assert.Same(c, resolveAll[2]);
        }
    }
}
