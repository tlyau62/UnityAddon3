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

    [Trait("DependencyExceptions", "CircularDependency/SelfLoop")]
    public class SelfLoopTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IHost Host { get; set; }

        [Fact]
        public void BeanDependencyValidatorStrategy_ResolveSelfLoopDependency_ExceptionThrown()
        {
            Assert.Throws<CircularDependencyException>(() => Host.PreInstantiateSingleton());
        }
    }
}
