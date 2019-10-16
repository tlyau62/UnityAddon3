using System;
using System.Collections.Generic;
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
    public class ResolveAllByPrimitiveTests
    {
        [Fact]
        public void ApplicationContext_ResolveAllByPrimitive_AllQualifiedBeanResolved()
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
