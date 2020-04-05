using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using Xunit;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [Trait("Aop", "MethodAttributeInterceptor")]
    public class GenericMethodAttributeInterceptorTests
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Logger Logger { get; set; }

        public GenericMethodAttributeInterceptorTests()
        {
            var host = new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace)
                .ConfigureUA<AopInterceptorContainerBuilder>(config =>
                {
                    config.AddAopIntercetor<IncInterceptor>();
                })
                .BuildUA()
                .RunTestUA(this);
        }

        [Fact]
        public void BeanInterceptionStrategy_GenericMethodInterceptors_InterceptorsAndTargetMethodAreExecuted()
        {
            Service.Inc("abc");

            Assert.Equal("testabc", Logger.Log);
        }

    }
}
