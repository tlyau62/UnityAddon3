using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Context;
using Xunit;
using UnityAddon.Core.Util.ComponentScanning;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [Configuration]
    public class GenericMethodAttributeConfig : AopInterceptorConfig
    {
        [Bean]
        public override AopInterceptorOption AopInterceptorOption()
        {
            var aopInterceptorOption = new AopInterceptorOption();

            aopInterceptorOption.AddAopIntercetor<IncInterceptor>();

            return aopInterceptorOption;
        }
    }

    [ComponentScan]
    public class GenericMethodAttributeInterceptorTests : UnityAddonTest
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        [Fact]
        public void GenericMethodAttributeInterceptor()
        {
            Service.Inc("abc");

            Assert.Equal("testabc", Logger.Log);
        }

    }
}
