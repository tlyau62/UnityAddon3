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
    /// Test case from https://github.com/unitycontainer/microsoft-dependency-injection/blob/master/tests/ScopedDepencencyTests.cs
    /// </summary>
    public class ScopeTransientTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void aspnet_Extensions_issues_1301()
        {
            IServiceProvider scopedSp1 = null;
            IServiceProvider scopedSp2 = null;
            Foo foo1 = null;
            Foo foo2 = null;

            using (var scope1 = Sp.CreateScope())
            {
                scopedSp1 = scope1.ServiceProvider;
                foo1 = scope1.ServiceProvider.GetRequiredService<Foo>();
            }

            using (var scope2 = Sp.CreateScope())
            {
                scopedSp2 = scope2.ServiceProvider;
                foo2 = scope2.ServiceProvider.GetRequiredService<Foo>();
            }

            Assert.Equal(foo1.ServiceProvider, foo2.ServiceProvider);
            Assert.NotEqual(foo1.ServiceProvider, scopedSp1);
            Assert.NotEqual(foo2.ServiceProvider, scopedSp2);
        }

        [Fact]
        public void ScopedDependencyFromFactoryNotSharedAcrossScopes()
        {
            // Act
            ITransient transient1a = null;
            ITransient transient1b = null;
            ITransient transient2b = null;

            using (var scope1 = Sp.CreateScope())
            {
                transient1a = scope1.ServiceProvider.GetService<ITransient>();
            }

            using (var scope2 = Sp.CreateScope())
            {
                transient1b = scope2.ServiceProvider.GetService<ITransient>();
                transient2b = scope2.ServiceProvider.GetService<ITransient>();
            }

            // Assert
            Assert.NotSame(transient1a, transient1b);
            Assert.NotSame(transient1b, transient2b);
            Assert.NotSame(transient1a.ScopedDependency, transient1b.ScopedDependency);
            Assert.Same(transient1b.ScopedDependency, transient2b.ScopedDependency);
        }

        [Fact]
        public void ScopedDependencyFromTransientNotSharedAcrossScopes()
        {
            // Act
            ITransient transient1a = null;
            ITransient transient1b = null;
            ITransient transient2b = null;

            using (var scope1 = Sp.CreateScope())
            {
                transient1a = scope1.ServiceProvider.GetService<ITransient>();
            }

            using (var scope2 = Sp.CreateScope())
            {
                transient1b = scope2.ServiceProvider.GetService<ITransient>();
                transient2b = scope2.ServiceProvider.GetService<ITransient>();
            }

            // Assert
            Assert.NotSame(transient1a, transient1b);
            Assert.NotSame(transient1b, transient2b);
            Assert.NotSame(transient1a.ScopedDependency, transient1b.ScopedDependency);
            Assert.Same(transient1b.ScopedDependency, transient2b.ScopedDependency);
        }

        [Component]
        public class Foo
        {
            public Foo(IServiceProvider sp)
            {
                ServiceProvider = sp;
            }

            public IServiceProvider ServiceProvider { get; }
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
