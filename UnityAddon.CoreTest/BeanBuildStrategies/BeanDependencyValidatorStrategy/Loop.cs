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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.Loop
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

    public class SelfLoopTests
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<IA, A>();
            serCol.AddSingleton<IB, B>();

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.Throws<CircularDependencyException>(() =>
            {
                sp.GetRequiredService<IA>();
                sp.GetRequiredService<IB>();
            });
        }
    }
}
