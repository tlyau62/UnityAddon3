using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using Xunit;
using Assert = Xunit.Assert;

namespace UnityAddon.CoreTest.Exceptions.NoUniqueBeanDefinition.NoSuchBeanDefinitionExceptionTests
{
    public interface IB { }

    [Component]
    public class Service
    {
        [Dependency]
        public IB B { get; set; }
    }

    [Trait("Exceptions", "NoSuchBeanDefinitionException")]
    public class NoSuchBeanDefinitionExceptionTests
    {
        [Fact]
        public void PropertyFill_NoSuchBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var ex = Assert.Throws<NoUniqueBeanDefinitionException>(() => new ApplicationContext(container, GetType().Namespace));

            Assert.Equal($"Property B in {typeof(Service).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }
    }

}
