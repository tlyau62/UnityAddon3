using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.PrimitiveBean
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

    [ComponentScan]
    public class PrimitiveBeanTests : UnityAddonTest
    {
        [Dependency]
        public Config Config { get; set; }

        [Dependency]
        public Service Service { get; set; }

        [Fact]
        public void PrimitiveBean()
        {
            Assert.Equal(Service.IntBean, Config.CreateIntBean());
            Assert.Same(Service.StringBean, Config.CreateStringBean());
        }
    }
}
