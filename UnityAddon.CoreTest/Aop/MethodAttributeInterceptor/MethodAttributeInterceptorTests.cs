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
using Xunit;
using Assert = Xunit.Assert;

namespace UnityAddon.CoreTest.Aop.MethodAttributeInterceptor
{
    [Trait("Aop", "MethodAttributeInterceptor")]
    public class MethodAttributeInterceptorTests
    {
        [Dependency]
        public IService Service { get; set; }

        [Dependency]
        public Counter Counter { get; set; }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public MethodAttributeInterceptorTests()
        {
            var host = new HostBuilder()
                .RegisterUA()
                .ScanComponentsUA(GetType().Namespace)
                .ConfigureUA<AopInterceptorContainerBuilder>(config =>
                {
                    config.AddAopIntercetor<IncInterceptor>();
                    config.AddAopIntercetor<MulInterceptor>();
                })
                .BuildUA()
                .BuildTestUA(this);
        }

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
            Assert.Same(Service, Container.ResolveUA<IService>());
        }

        [Fact]
        public void BeanInterceptionStrategy_ProxyOfDifferentInterfaces_ProxyStateIsConsistent()
        {
            var s1 = Container.ResolveUA<ISetDep>();
            var s2 = Container.ResolveUA<ISetDep2>();

            Assert.Same(s1, s2);
        }
    }
}
