using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.GenericBean.GenericDefinitionBean
{
    public interface IService<T> { }

    [Component]
    public class Service<T> : IService<T> { }

    [Trait("Bean", "GenericBean/GenericDefinitionBean")]
    public class GenericDefinitionBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnGenericDefinitionBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var service = appContext.Resolve<IService<int>>();

            Assert.IsType<Service<int>>(service);
        }
    }
}
