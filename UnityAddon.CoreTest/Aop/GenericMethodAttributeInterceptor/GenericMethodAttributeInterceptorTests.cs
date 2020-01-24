using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using Xunit;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [Trait("Aop", "MethodAttributeInterceptor")]
    public class GenericMethodAttributeInterceptorTests
    {
        private IService _service;
        private ApplicationContext _appContext;

        public GenericMethodAttributeInterceptorTests()
        {
            _appContext = new ApplicationContext(new UnityContainer(), GetType().Namespace);
            _service = _appContext.Resolve<IService>();
        }

        [Fact]
        public void BeanInterceptionStrategy_GenericMethodInterceptors_InterceptorsAndTargetMethodAreExecuted()
        {
            _service.Inc("abc");

            Assert.Equal("testabc", _appContext.Resolve<Logger>().Log);
        }

    }
}
