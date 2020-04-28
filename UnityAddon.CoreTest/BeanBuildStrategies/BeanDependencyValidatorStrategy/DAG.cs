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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.DAG
{
    [Component]
    class D
    {
        public D(E e) { }
    }

    [Component]
    class E
    {
        public E(F f) { }
    }

    [Component]
    class F
    {
    }

    public class DAG
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();
            var types = new[] { typeof(D), typeof(E), typeof(F) };

            foreach (var type in types)
            {
                serCol.AddSingleton(type);
            }

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            foreach (var type in types)
            {
                sp.GetRequiredService(type);
            }
        }
    }
}
