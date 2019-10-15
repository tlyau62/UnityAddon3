using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Dependency.Bean.ProfileBean
{
    interface IService
    {
    }

    [Component]
    [Profile("dev")]
    class DevService : IService
    {
    }

    [Component]
    [Profile("prod")]
    class ProdService : IService
    {
    }

    [Trait("Bean", "ProfileBean")]
    public class ProfileBeanTests
    {
        [Theory]
        [InlineData("prod", typeof(ProdService))]
        [InlineData("dev", typeof(DevService))]
        public void BuildStrategy_DependencyOnProfileBean_BeanInjected(string activeProfile, Type resolveType)
        {
            var container = new UnityContainer();
            container.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"profiles:active", activeProfile},
                })
                .Build());

            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.IsType(resolveType, appContext.Resolve<IService>());
        }
    }
}
