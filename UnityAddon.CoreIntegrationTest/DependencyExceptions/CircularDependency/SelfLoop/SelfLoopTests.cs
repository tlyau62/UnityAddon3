using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.SelfLoop
{
    [Component]
    class A
    {
        public A(A a) { }
    }

    [ComponentScan(typeof(SelfLoopTests))]
    public class SelfLoopTests : UnityAddonTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void SelfLoop()
        {
            Assert.Throws<CircularDependencyException>(() => Sp.GetRequiredService<A>());
        }
    }
}
