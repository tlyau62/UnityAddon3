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
    public class GenericDependencyTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IService<int> IntService { get; set; }

        [Dependency]
        public IService<string> StringService { get; set; }

        [Dependency]
        public MainService MainService { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void BuildStrategy_DependencyOnGenericTypeBean_BeanInjected()
        {
            Assert.Same(IntService, MainService.IntService);
            Assert.Same(StringService, MainService.StringService);

            Assert.IsType<IntService>(IntService);
            Assert.IsType<Service<string>>(StringService);
        }

        [Fact]
        public void BuildStrategy_CheckOnGenericTypeBeanIsRegistered_TrueReturned()
        {
            Assert.True(UnityContainer.IsRegisteredUA<IntService>());
            Assert.True(UnityContainer.IsRegisteredUA<IService<int>>());
            Assert.True(UnityContainer.IsRegisteredUA<IService<string>>());
            Assert.True(UnityContainer.IsRegisteredUA<IService<double>>());
            Assert.True(UnityContainer.IsRegisteredUA(typeof(IService<>)));
        }
    }

}
