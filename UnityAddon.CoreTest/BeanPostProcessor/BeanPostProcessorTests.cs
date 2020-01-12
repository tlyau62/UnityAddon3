using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanPostprocessor;
using Xunit;

namespace UnityAddon.CoreTest.BeanPostProcessor
{
    [Component]
    public class Logger
    {
        public List<string> Logs { get; set; } = new List<string>();
    }

    [Component]
    public class LoggerPostProcessor : IBeanPostProcessor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void PostProcess(object bean, string beanName)
        {
            Logger.Logs.Add(bean.GetType().Name);
        }
    }

    [Component]
    public class Service
    {
    }

    [Trait("BeanPostProcessor", "BeanPostProcessor")]
    public class BeanPostProcessorTests
    {
        [Fact]
        public void BeanPostProcessor_ServiceBeanLogging_ServiceBeanLogged()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            Assert.Contains(nameof(Service), appContext.Resolve<Logger>().Logs);
        }
    }
}
