using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;
using Unity.Lifetime;

namespace UnityAddonTest.Resolve.ResolveAll
{
    public interface IService { }

    [Component]
    [Qualifier("a")]
    public class A : IService { }

    [Component]
    [Qualifier("b")]
    public class B : IService { }

    public class C : IService { }

    [Trait("Resolve", "ResolveAllByInterfaces")]
    public class ResolveAllByInterfacesTests
    {
        [Fact]
        public void ApplicationContext_ResolveAllByInterfaces_BeanResolved()
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

namespace UnityAddonTest.Resolve.ResolveAll
{
    [Configuration]
    public class AppConfig
    {
        [Bean]
        [Qualifier("a")]
        public virtual string CreateStrA()
        {
            return "A";
        }

        [Bean]
        [Qualifier("b")]
        public virtual string CreateStrB()
        {
            return "B";
        }
    }

    [Trait("Resolve", "ResolveAllByPrimitive")]
    public class ResolveAllByPrimitiveTests
    {
        [Fact]
        public void ApplicationContext_ResolveAllByPrimitive_BeanResolved()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var a = appContext.Resolve<string>("a");
            var b = appContext.Resolve<string>("b");

            var resolveAll = appContext.ResolveAll<string>();

            Assert.Same(a, resolveAll[0]);
            Assert.Same(b, resolveAll[1]);
        }
    }
}
