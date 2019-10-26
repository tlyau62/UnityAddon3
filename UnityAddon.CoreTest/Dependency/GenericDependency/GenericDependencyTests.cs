using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Dependency.GenericDependency
{
    public interface IService<T> { }

    [Component]
    public class Service<T> : IService<T> { }

    [Component]
    public class IntService : IService<int> { }

    [Component]
    public class MainService
    {
        [Dependency]
        public IService<int> IntService { get; set; }

        [Dependency]
        public IService<string> StringService { get; set; }
    }

    [Trait("Dependency", "GenericDependency")]
    public class GenericDependencyTests
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

            Assert.IsType<IntService>(intService);
            Assert.IsType<Service<string>>(stringService);
        }

        [Fact]
        public void BuildStrategy_CheckOnGenericTypeBeanIsRegistered_TrueReturned()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            Assert.True(appContext.IsRegistered<IntService>());
            Assert.True(appContext.IsRegistered<IService<int>>());
            Assert.True(appContext.IsRegistered<IService<string>>());
            Assert.True(appContext.IsRegistered<IService<double>>());
            Assert.True(appContext.IsRegistered(typeof(IService<>)));
        }
    }

}
