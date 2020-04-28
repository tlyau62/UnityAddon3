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

namespace UnityAddon.CoreTest.BeanBuildStrategies.BeanDependencyValidatorStrategy.DAG2
{
    [Component]
    class G
    {
        public G(I i) { }
    }

    [Component]
    class H
    {
        public H(I i) { }
    }

    [Component]
    class I
    {
    }

    public class DAG2
    {
        [Fact]
        public void Test()
        {
            var serCol = new ServiceCollection();
            var types = new[] { typeof(G), typeof(H), typeof(I) };

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
