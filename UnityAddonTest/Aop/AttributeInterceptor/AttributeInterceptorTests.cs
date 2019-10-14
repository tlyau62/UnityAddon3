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
        private IUnityContainer _container;
        private IService _service;

        public AttributeInterceptorTests()
        {
            _container = new UnityContainer();
            var appContext = new ApplicationContext(_container, GetType().Namespace);

            _service = _container.Resolve<IService>();
        }

        [Fact]
        public void BeanInterceptionStrategy_ChainInterceptors_AllInterceptorsAndTargetMethodAreExecuted()
        {
            _service.ChainInterceptedServe();

            Assert.Equal(3, _container.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodWithinTheSameClass_CalleeInterceptorsAreNotExecuted()
        {
            _service.CallMethodsInsideSameService();

            Assert.Equal(3, _container.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_TargetMethodCallTheMethodInAnotherClass_CalleeInterceptorsAreExecuted()
        {
            _service.CallMethodsOutsideService();

            Assert.Equal(8, _container.Resolve<Counter>().Count);
        }

        [Fact]
        public void BeanInterceptionStrategy_SingletonProxy_ReturnCachedProxy()
        {
            Assert.Same(_service, _container.Resolve<IService>());
        }

        [Fact]
        public void BeanInterceptionStrategy_ProxyOfDifferentInterfaces_ProxyStateIsConsistent()
        {
            var s1 = _container.Resolve<ISetDep>();
            var s2 = _container.Resolve<ISetDep2>();

            Assert.Same(s1.CounterAccess(), s2.CounterAccess2());
        }
    }
}
