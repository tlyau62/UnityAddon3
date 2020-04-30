using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.DependencyInjection
{
    [Component]
    public class CtorHelper { }

    [Component]
    public class PropertyHelper { }

    public class Service
    {
        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        public CtorHelper CtorHelper { get; set; }

        public Service(CtorHelper ctorHelper)
        {
            CtorHelper = ctorHelper;
        }
    }

    [Configuration]
    public class Config
    {
        [Bean]
        public virtual Service CreateService(CtorHelper ctorHelper)
        {
            return new Service(ctorHelper);
        }
    }

    public class DependencyInjectionTests : DefaultTest
    {
        [Dependency]
        public Service Service { get; set; }

        [Dependency]
        public CtorHelper CtorHelper { get; set; }

        [Dependency]
        public PropertyHelper PropertyHelper { get; set; }

        [Fact]
        public void DependencyInjection()
        {
            Assert.Same(CtorHelper, Service.CtorHelper);
            Assert.Null(Service.PropertyHelper);
        }
    }
}
