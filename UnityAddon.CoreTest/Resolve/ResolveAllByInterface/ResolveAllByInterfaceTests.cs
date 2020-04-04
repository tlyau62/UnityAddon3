using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core.Attributes;
using Xunit;
using Unity.Lifetime;
using UnityAddon.Core;
using System.Linq;

namespace UnityAddon.CoreTest.Resolve.ResolveAllByInterface
{
    public interface IService { }

    [Component]
    [Qualifier("a")]
    public class A : IService { }

    [Component]
    [Qualifier("b")]
    public class B : IService { }

    public class C : IService { }

    public class D : IService { }

    [Trait("Resolve", "ResolveAllByInterface")]
    public class ResolveAllByInterfaceTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void ApplicationContext_ResolveAllByInterface_AllQualifiedBeanResolved()
        {
            var a = UnityContainer.ResolveUA<IService>("a");
            var b = UnityContainer.ResolveUA<IService>("b");
            var c = UnityContainer
                .RegisterTypeUA<IService, C>("c", new SingletonLifetimeManager())
                .ResolveUA<IService>("c");
            var d = UnityContainer
                .RegisterTypeUA<IService, D>(new SingletonLifetimeManager())
                .ResolveUA<IService>(); // bean name is null

            var resolveAll = UnityContainer.ResolveAllUA<IService>().ToArray();

            Assert.Same(a, resolveAll[0]);
            Assert.Same(b, resolveAll[1]);
            Assert.Same(c, resolveAll[2]);
            Assert.Same(d, resolveAll[3]);
        }
    }
}
