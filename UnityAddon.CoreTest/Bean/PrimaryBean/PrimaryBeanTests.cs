using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.PrimaryBean
{
    public interface IService
    {
    }

    [Component]
    public class ServiceA : IService { }

    [Component]
    [Primary]
    public class ServiceB : IService { }

    [Trait("Bean", "PrimaryBean")]
    public class PrimaryBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnPrimaryBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var primaryB = appContext.Resolve<IService>();

            Assert.IsType<ServiceB>(primaryB);
        }
    }
}
