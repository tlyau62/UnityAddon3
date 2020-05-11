using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Configs;
using Xunit;

namespace UnityAddon.CoreTest.Bean.QualifiedBean
{
    public interface ICommon { }
    public interface IRare { }

    public enum ServiceType
    {
        Popular
    }

    [Component]
    public class Service
    {
        [Dependency("CommonB")]
        public ICommon BCommon { get; set; }

        [Dependency]
        public IRare BRare { get; set; }

        [Dependency("A")]
        public ICommon A { get; set; }

        [Dependency("Popular")]
        public ICommon C { get; set; }
    }

    [Component]
    [Qualifier("CommonA")]
    public class A : ICommon
    {
    }

    [Component]
    [Qualifier("CommonB")]
    public class B : ICommon, IRare
    {
    }

    [Component]
    [Qualifier(ServiceType.Popular)]
    public class C : ICommon
    {
    }

    [ConfigArg("csconfig_testcase", typeof(QualifiedBeanTests))]
    [Import(typeof(ComponentScanTestConfig))]
    public class QualifiedBeanTests : UnityAddonTest
    {
        [Dependency("CommonA")]
        public ICommon A { get; set; }

        [Dependency("CommonB")]
        public ICommon B { get; set; }

        [Dependency(nameof(ServiceType.Popular))]
        public ICommon C { get; set; }

        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void QualifiedBean()
        {
            Assert.Same(Service.BCommon, B);
            Assert.Same(Service.BRare, B);
            Assert.Same(Service.A, A);
            Assert.Same(Service.C, C);
        }
    }
}
