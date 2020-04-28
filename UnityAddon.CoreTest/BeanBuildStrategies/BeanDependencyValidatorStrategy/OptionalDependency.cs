using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.OptionalDependency
{
    [Component]
    class D
    {
        [Dependency]
        public A A { get; set; }

        [OptionalDependency]
        public B B { get; set; }
    }

    [Component]
    class A
    {
        [OptionalDependency]
        public B B { get; set; }

        [OptionalDependency]
        public C C { get; set; }
    }

    class B
    {
    }

    class C
    {
    }

    public class OptionalDependency
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();

            serCol.AddSingleton<A>();
            serCol.AddSingleton<B>();
            serCol.AddSingleton<C>();
            serCol.AddSingleton<D>();

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            sp.GetRequiredService<A>();
            sp.GetRequiredService<B>();
            sp.GetRequiredService<C>();
            sp.GetRequiredService<D>();
        }
    }
}
