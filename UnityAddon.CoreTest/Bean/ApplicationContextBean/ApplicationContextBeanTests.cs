using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using Xunit;

namespace UnityAddon.CoreTest.Bean.ApplicationContextBean
{
    [Trait("Bean", "ApplicationContextBean")]
    public class ApplicationContextBeanTests
    {
        [Fact]
        public void ApplicationContext_ApplicationContextBean_SingletonBeanReturned()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Same(appContext, appContext.Resolve<ApplicationContext>());
        }
    }
}
