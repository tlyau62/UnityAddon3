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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.ComplexLoop
{
    [Component]
    class M1
    {
        public M1(M2 m2) { }
    }

    [Component]
    class M2
    {
        public M2(M3 m3, M4 m4, M5 m5) { }
    }

    [Component]
    class M3
    {
        public M3(M5 m5) { }
    }

    [Component]
    class M4
    {
        public M4(M5 m5) { }
    }

    [Component]
    class M5
    {
        public M5(M6 m6) { }
    }

    [Component]
    class M6
    {
        public M6(M7 m7) { }
    }

    [Component]
    class M7
    {
        public M7(M4 m4) { }
    }

    public class ComplexLoop
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();
            var types = new[] { typeof(M1), typeof(M2), typeof(M3), typeof(M4), typeof(M5), typeof(M6), typeof(M7) };

            foreach (var type in types)
            {
                serCol.AddSingleton(type);
            }

            var sp = new ServiceProviderFactory().CreateBuilder(serCol).Resolve<IServiceProvider>();

            Assert.Throws<CircularDependencyException>(() =>
            {
                foreach (var type in types)
                {
                    sp.GetRequiredService(type);
                }
            });
        }
    }
}
