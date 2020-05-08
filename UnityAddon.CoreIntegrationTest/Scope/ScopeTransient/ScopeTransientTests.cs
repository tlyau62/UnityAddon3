using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreIntegrationTest.Scope.ScopeTransient
{
    /// <summary>
    /// Test case from https://github.com/unitycontainer/microsoft-dependency-injection/commit/09f0f0715199c3906f2939fb62bc10dab2e718b7
    /// </summary>
    public class ScopeTransientTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void ScopedDependencyFromTransientFactoryNotSharedAcrossScopes()
        {
            // Act
            ITransient transient1 = null;
            ITransient transient2a = null;
            ITransient transient2b = null;

            using (var scope1 = Sp.CreateScope())
            {
                transient1 = scope1.ServiceProvider.GetService<ITransient>();
            }

            using (var scope2 = Sp.CreateScope())
            {
                transient2a = scope2.ServiceProvider.GetService<ITransient>();
                transient2b = scope2.ServiceProvider.GetService<ITransient>();
            }

            // Assert
            Assert.NotSame(transient1, transient2a);
            Assert.NotSame(transient2a, transient2b);
            Assert.NotSame(transient1.ScopedDependency, transient2a.ScopedDependency);
            Assert.Same(transient2a.ScopedDependency, transient2b.ScopedDependency);
        }

        public interface ITransient
        {
            IScoped ScopedDependency { get; }
        }

        [Component]
        [Scope(ScopeType.Transient)]
        public class Transient : ITransient
        {
            public Transient(IScoped scoped)
            {
                ScopedDependency = scoped;
            }

            public IScoped ScopedDependency { get; }
        }

        public interface IScoped { }

        [Component]
        [Scope(ScopeType.Scoped)]
        public class Scoped : IScoped
        {
        }

    }
}
