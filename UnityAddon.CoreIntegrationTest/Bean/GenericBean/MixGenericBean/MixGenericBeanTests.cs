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

    public class MixGenericBeanTests : UnityAddonComponentScanTest
    {
        [Dependency]
        public IService<int> IntService { get; set; }

        [Dependency]
        public IService<string> StringService { get; set; }

        [Dependency]
        public MainService MainService { get; set; }

        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void GenericTypeBean()
        {
            Assert.Same(IntService, MainService.IntService);
            Assert.Same(StringService, MainService.StringService);

            Assert.IsType<IntService>(IntService);
            Assert.IsType<Service<string>>(StringService);
        }

        [Fact]
        public void GenericTypeBean_IsRegistered()
        {
            Assert.True(Sp.IsRegistered<IntService>());
            Assert.True(Sp.IsRegistered<IService<int>>());
            Assert.True(Sp.IsRegistered<IService<string>>());
            Assert.True(Sp.IsRegistered<IService<double>>());
            Assert.True(Sp.IsRegistered(typeof(IService<>)));
        }
    }

}
