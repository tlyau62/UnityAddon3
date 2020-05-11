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

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
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
                .ConfigureContainer<ApplicationContext>(ctx =>
                {
                    ctx.ConfigureBeans((config, sp) => config.AddFromComponentScanner(GetType().Assembly, GetType().Namespace));
                    ctx.ConfigureContext<AopInterceptorContainerOption>(option =>
                    {
                        option.AddAopIntercetor<IncInterceptor>();
                    });
                })
                .Build();

            ((IUnityAddonSP)host.Services).BuildUp(this);
        }

        [Fact]
        public void GenericMethodAttributeInterceptor()
        {
            Service.Inc("abc");

            Assert.Equal("testabc", Logger.Log);
        }

    }
}
