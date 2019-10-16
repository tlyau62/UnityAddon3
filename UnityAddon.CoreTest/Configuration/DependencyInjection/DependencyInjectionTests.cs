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

    [Trait("Configuration", "DependencyInjection")]
    public class DependencyInjectionTests
    {
        [Fact]
        public void BuildStrategy_BeanMethodDependency_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var service = appContext.Resolve<Service>();
            var ctorHelper = appContext.Resolve<CtorHelper>();
            var propertyHelper = appContext.Resolve<PropertyHelper>();

            Assert.Same(ctorHelper, service.CtorHelper);
            Assert.Same(propertyHelper, service.PropertyHelper);
        }
    }
}
