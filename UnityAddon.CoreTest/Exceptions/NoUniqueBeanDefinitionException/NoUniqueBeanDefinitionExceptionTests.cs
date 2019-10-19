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

namespace UnityAddon.CoreTest.Exceptions.NoUniqueBeanDefinition.NoUniqueBeanDefinitionExceptionTests
{
    public interface IB { }

    [Component]
    public class B1 : IB { }

    [Component]
    public class B2 : IB { }

    [Component]
    public class Service
    {
        [Dependency]
        public IB B { get; set; }
    }

    [Trait("Exceptions", "NoUniqueBeanDefinitionException")]
    public class NoUniqueBeanDefinitionExceptionTests
    {
        [Fact]
        public void PropertyFill_NoUniqueBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var ex = Assert.Throws<NoUniqueBeanDefinitionException>(() => new ApplicationContext(container, GetType().Namespace));

            Assert.Equal($"Property B in {typeof(Service).FullName} required a single bean, but 2 were found:\r\n" +
                $"- {typeof(B1).Name}: defined in namespace [{typeof(B1).Namespace}]\r\n" +
                $"- {typeof(B2).Name}: defined in namespace [{typeof(B2).Namespace}]", ex.Message);
        }
    }

}
