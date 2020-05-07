using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.OptionalDependency
{
    [Component]
    public class Helper
    {
    }

    public class CustHelper
    {
    }

    public interface ITest { }

    [Component]
    public class Service
    {
        [OptionalDependency]
        public Helper Helper { get; set; }

        [OptionalDependency]
        public CustHelper CustHelper { get; set; }

        [OptionalDependency]
        public ITest Test { get; set; }
    }

    public class OptionalDependencyTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void OptionalDependency()
        {
            Assert.NotNull(Service.Helper);
        }

        [Fact]
        public void OptionalDependencyOnClass_BeanNotFound()
        {
            Assert.Null(Service.CustHelper);
        }

        [Fact]
        public void OptionalDependencyOnInterface_BeanNotFound()
        {
            Assert.Null(Service.Test);
        }
    }
}
