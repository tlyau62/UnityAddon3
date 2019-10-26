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
        public void BuildStrategy_DependencyOnGenericTypeBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var service = appContext.Resolve<IService<int>>();

            Assert.IsType<Service<int>>(service);
        }
    }
}

namespace UnityAddon.CoreTest.Bean.GenericBean.ConcreteGenericBean
{
    public interface IService<T> { }

    [Component]
    public class Service : IService<int> { }

    [Trait("Bean", "GenericBean/ConcreteGenericBean")]
    public class ConcreteGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnGenericTypeBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var service = appContext.Resolve<IService<int>>();

            Assert.IsType<Service>(service);
        }
    }
}

namespace UnityAddon.CoreTest.Bean.GenericBean.GenericDependency
{
    public interface IService<T> { }

    [Component]
    public class IntService : IService<int> { }

    [Component]
    public class Service<T> : IService<T> { }

    [Component]
    public class MainService
    {
        [Dependency]
        public IService<int> IntService { get; set; }

        [Dependency]
        public IService<string> StringService { get; set; }
    }

    [Trait("Dependency", "GenericDependency")]
    public class ConcreteGenericBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnGenericTypeBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var intService = appContext.Resolve<IService<int>>();
            var stringService = appContext.Resolve<IService<string>>();
            var mainService = appContext.Resolve<MainService>();

            Assert.Same(intService, mainService.IntService);
            Assert.Same(stringService, mainService.StringService);
        }
    }
}