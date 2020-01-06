using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GenericBean.ConcreteGenericBean
{
    public interface IService<T> { }

    [Component]
    public class Service : IService<int> { }

    [Trait("Bean", "GenericBean/ConcreteGenericBean")]
    public class ConcreteGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnConcreteGenericBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var service = appContext.Resolve<IService<int>>();

            Assert.IsType<Service>(service);
            Assert.True(appContext.IsRegistered<IService<int>>());
        }
    }
}