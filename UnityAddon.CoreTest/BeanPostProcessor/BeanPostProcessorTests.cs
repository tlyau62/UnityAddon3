using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
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
    [Order(1)]
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
    [Order(Ordered.LOWEST_PRECEDENCE)]
    public class PostLoggerPostProcessor : IBeanPostProcessor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void PostProcess(object bean, string beanName)
        {
            Logger.Logs.Add("post: " + bean.GetType().Name);
        }
    }

    [Component]
    [Order(Ordered.HIGHEST_PRECEDENCE)]
    public class PreLoggerPostProcessor : IBeanPostProcessor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void PostProcess(object bean, string beanName)
        {
            Logger.Logs.Add("pre: " + bean.GetType().Name);
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

        [Fact]
        public void BeanPostProcessor_ServiceBeanPrePostLogging_PrePostServiceBeanLogged()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);
            var logger = appContext.Resolve<Logger>();

            for (var i = 2; i < logger.Logs.Count; i += 3)
            {
                var beanName = logger.Logs[i - 1];

                Assert.Equal("pre: " + beanName, logger.Logs[i - 2]);
                Assert.Equal("post: " + beanName, logger.Logs[i]);
            }
        }
    }
}
