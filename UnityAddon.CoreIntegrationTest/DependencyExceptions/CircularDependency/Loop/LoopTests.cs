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

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.Loop
{
    public interface IA { }
    public interface IB { }

    [Component]
    class A : IA
    {
        public A(IB b) { }
    }

    [Component]
    class B : IB
    {
        public B(IA a) { }
    }

    [ComponentScan(typeof(SelfLoopTests))]
    public class SelfLoopTests : UnityAddonTest
    {
        public SelfLoopTests() : base(true)
        {
        }

        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveLoopDependency_ExceptionThrown()
        {
            Assert.Throws<CircularDependencyException>(() => HostBuilder.Build());
        }
    }
}
