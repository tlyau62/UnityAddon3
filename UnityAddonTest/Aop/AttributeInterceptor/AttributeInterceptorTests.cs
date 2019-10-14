using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon;
using Xunit;
using Assert = Xunit.Assert;

namespace UnityAddonTest.Aop.AttributeInterceptor
{
    [Trait("Aop", "AttributeInterceptor")]
    public class AttributeInterceptorTests
    {
        private IService _service;
        private ApplicationContext _appContext;

        public AttributeInterceptorTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace);
            _service = _appContext.Resolve<IService>();
        }

        [Fact]
        public void BeanInterceptionStrategy_ChainInterceptors_AllInterceptorsAndTargetMethodAreExecuted()
        {
            _service.ChainInterceptedServe();

            Assert.Equal(3, _appContext.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodWithinTheSameClass_CalleeInterceptorsAreNotExecuted()
        {
            _service.CallMethodsInsideSameService();

            Assert.Equal(3, _appContext.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodInAnotherClass_CalleeInterceptorsAreExecuted()
        {
            _service.CallMethodsOutsideService();

            Assert.Equal(8, _appContext.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_SingletonProxy_ReturnCachedProxy()
        {
            Assert.Same(_service, _appContext.Resolve<IService>());
        }

        [Fact]
        public void BeanInterceptionStrategy_ProxyOfDifferentInterfaces_ProxyStateIsConsistent()
        {
            var s1 = _appContext.Resolve<ISetDep>();
            var s2 = _appContext.Resolve<ISetDep2>();

            Assert.Same(s1, s2);
        }
    }
}
