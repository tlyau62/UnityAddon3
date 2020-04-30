using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Resolve.ResolveAllByPrimitive
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
    public class ResolveAllByPrimitiveTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void ApplicationContext_ResolveAllByPrimitive_AllQualifiedBeanResolved()
        {
            var a = UnityContainer.ResolveUA<string>("a");
            var b = UnityContainer.ResolveUA<string>("b");

            var resolveAll = UnityContainer.ResolveAllUA<string>().ToArray();

            Assert.Same(a, resolveAll[0]);
            Assert.Same(b, resolveAll[1]);
        }
    }
}
