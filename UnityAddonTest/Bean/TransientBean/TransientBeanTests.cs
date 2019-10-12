using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Bean.TransientBean
{
    [Component]
    [Scope(ScopeType.Transient)]
    public class Helper
    {
    }

    [Component]
    public class Service
    {
        [Dependency]
        public Helper Helper { get; set; }

        [Dependency]
        public Helper Helper2 { get; set; }
    }

    [Trait("Bean", "TransientBean")]
    public class TransientBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnTransientBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var service = appContext.Resolve<Service>();

            Assert.NotSame(service.Helper, service.Helper2);
        }
    }
}
