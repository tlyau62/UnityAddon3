using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Context;
using UnityAddon.Core.Util.ComponentScanning;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Assert = Xunit.Assert;
using UnityAddon.Core.Attributes;

namespace UnityAddon.CoreTest.Aop.MethodAttributeInterceptor
{
    [Configuration]
    public class MethodAttributeInterceptorConfig : AopInterceptorConfig
    {
        [Bean]
        public override AopInterceptorOption AopInterceptorOption()
        {
            var aopInterceptorOption = new AopInterceptorOption();

            aopInterceptorOption.AddAopIntercetor<IncInterceptor>();
            aopInterceptorOption.AddAopIntercetor<MulInterceptor>();

            return aopInterceptorOption;
        }
    }

    [ComponentScan]
    public class MethodAttributeInterceptorTests : UnityAddonTest
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Counter Counter { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Fact]
        public void BeanInterceptionStrategy_ChainInterceptors_AllInterceptorsAndTargetMethodAreExecuted()
        {
            Service.ChainInterceptedServe();

            Assert.Equal(3, Counter.Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodWithinTheSameClass_CalleeInterceptorsAreNotExecuted()
        {
            Service.CallMethodsInsideSameService();

            Assert.Equal(3, Counter.Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodInAnotherClass_CalleeInterceptorsAreExecuted()
        {
            Service.CallMethodsOutsideService();

            Assert.Equal(8, Counter.Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_SingletonProxy_ReturnCachedProxy()
        {
            Assert.Same(Service, Sp.GetRequiredService<IService>());
        }

        [Fact]
        public void BeanInterceptionStrategy_ProxyOfDifferentInterfaces_ProxyStateIsConsistent()
        {
            var s1 = Sp.GetRequiredService<ISetDep>();
            var s2 = Sp.GetRequiredService<ISetDep2>();

            Assert.Same(s1, s2);
        }
    }
}
