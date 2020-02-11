using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B;
using Xunit;

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.A
{
    [Component]
    public class ServiceA : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.B
{
    [Component]
    public class ServiceB : ISerivce
    {
    }
}

namespace UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter
{
    public interface ISerivce { }

    [Trait("ComponentScan", "NamespaceExclude")]
    public class NamespaceFilterTests
    {
        [Fact]
        public void BeanDefinitionRegistry_ComponentScanNamespaceExcludeFilter_TargetNamespaceExluced()
        {
            var unityContainer = new UnityContainer();
            var excludeFilter = new ComponentScanNamespaceExcludeFilter("UnityAddon.CoreTest.ComponentScan.NamespaceExcludeFilter.A");

            unityContainer.RegisterInstance(excludeFilter);

            var appCtx = new ApplicationContext(unityContainer, GetType().Namespace);
            var service = appCtx.Resolve<ISerivce>();

            Assert.IsType<ServiceB>(service);
        }
    }
}
