using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.ConcreteGenericBean
{
    public interface IService<T> { }

    public class Person { }

    [Component]
    class Service : IService<Person> { }

    [Trait("Bean", "ConcreteGenericBean")]
    public class ConcreteGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnConcreteGenericTypeBean_BeanInjected()
        {
            IUnityContainer container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            Assert.IsType<Service>(appContext.Resolve<IService<Person>>());
        }
    }
}
