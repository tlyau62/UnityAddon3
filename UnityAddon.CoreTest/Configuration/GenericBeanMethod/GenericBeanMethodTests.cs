using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Configuration.GenericBeanMethod
{
    [Configuration]
    public class Config
    {
        [Bean]
        public virtual List<int> IntList()
        {
            return new List<int>();
        }

        [Bean]
        public virtual IEnumerable<string> StrList()
        {
            return new List<string>();
        }
    }

    [Trait("Configuration", "GenericBeanMethod")]
    public class GenericBeanMethodTests
    {
        [Fact]
        public void BeanMethodInterceptor_GenericBeanMethod_BeanInjected()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var a = appContext.Resolve<List<int>>();
            var b = appContext.Resolve<IEnumerable<string>>();

            Assert.IsType<List<int>>(a);
            Assert.IsType<List<string>>(b);
        }
    }
}
