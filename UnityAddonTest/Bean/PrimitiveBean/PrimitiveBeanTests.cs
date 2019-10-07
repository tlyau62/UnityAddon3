using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;
using Xunit;

namespace UnityAddonTest.Bean.PrimitiveBean
{
    [Configuration]
    public class Config
    {
        [Bean]
        public virtual string CreateStringBean()
        {
            return "test";
        }

        [Bean]
        public virtual int CreateIntBean()
        {
            return 10;
        }
    }

    [Component]
    public class Service
    {
        [Dependency]
        public string StringBean { get; set; }

        [Dependency]
        public int IntBean { get; set; }
    }

    [Trait("Bean", "PrimitiveBean")]
    public class PrimitiveBeanTests
    {
        [Fact]
        public void BuildStrategy_DependencyOnPrimitiveBean_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var config = container.Resolve<Config>();
            var service = container.Resolve<Service>();

            Assert.Equal(service.IntBean, config.CreateIntBean());
            Assert.Same(service.StringBean, config.CreateStringBean());
        }
    }
}
