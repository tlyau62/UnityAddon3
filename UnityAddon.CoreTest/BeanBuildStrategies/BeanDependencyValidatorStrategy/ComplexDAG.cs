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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.ComplexDAG
{
    class N1
    {
        public N1(N2 n2) { }
    }

    class N2
    {
        public N2(N3 n3, N4 n4, N5 n5) { }
    }

    class N3
    {
        public N3(N5 n5) { }
    }

    class N4
    {
        public N4(N5 n5) { }
    }

    class N5
    {
        public N5(N6 n6) { }
    }

    class N6
    {
        public N6() { }
    }

    class N7
    {
        public N7(N4 n4) { }
    }

    public class ComplexDAG
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();
            var types = new[] { typeof(N1), typeof(N2), typeof(N3), typeof(N4), typeof(N5), typeof(N6), typeof(N7) };

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
