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

    public class LoggerPostProcessor : IBeanPostProcessor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void PostProcess(object bean, string beanName)
        {
            Logger.Logs.Add(bean.GetType().Name);
        }
    }

    [Trait("BeanPostProcessor", "BeanPostProcessor")]
    public class BeanPostProcessorTests
    {
        [Fact]
        public void BeanPostProcessor_BeanTypeLogging_AllBeanTypeLogged()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            appContext.RegisterBeanPostProcessors(new List<IBeanPostProcessor>() { appContext.BuildUp(new LoggerPostProcessor()) });

            appContext.PreInstantiateSingleton();

            Assert.NotEmpty(appContext.Resolve<Logger>().Logs);
        }
    }
}
