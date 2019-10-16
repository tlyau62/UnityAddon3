using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.QualifiedBean
{
    public interface ICommon { }
    public interface IRare { }

    [Component]
    public class Service
    {
        [Dependency("CommonB")]
        public ICommon BCommon { get; set; }

        [Dependency]
        public IRare BRare { get; set; }

        [Dependency("A")]
        public ICommon A { get; set; }
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

    [Trait("Bean", "QualifiedBean")]
    public class QualifiedBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnQualifiedBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var a = appContext.Resolve<ICommon>("CommonA");
            var b = appContext.Resolve<ICommon>("CommonB");
            var service = appContext.Resolve<Service>();

            Assert.Same(service.BCommon, b);
            Assert.Same(service.BRare, b);
            Assert.Same(service.A, a);
        }
    }
}
