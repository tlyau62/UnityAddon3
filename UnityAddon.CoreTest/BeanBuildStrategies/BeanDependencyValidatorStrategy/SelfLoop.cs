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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.SelfLoop
{
    class A
    {
        public A(A a) { }
    }

    public class SelfLoop
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<A>();

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.Throws<CircularDependencyException>(() => sp.GetRequiredService<A>());
        }
    }
}
