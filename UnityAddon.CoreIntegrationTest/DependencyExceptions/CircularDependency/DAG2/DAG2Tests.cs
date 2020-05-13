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

namespace UnityAddon.CoreTest.DependencyExceptions.CircularDependency.DAG2
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

    [ComponentScan]
    public class DAG2Tests : UnityAddonTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void DAG2()
        {
            Sp.GetRequiredService<G>();
            Sp.GetRequiredService<H>();
            Sp.GetRequiredService<I>();
        }
    }
}
