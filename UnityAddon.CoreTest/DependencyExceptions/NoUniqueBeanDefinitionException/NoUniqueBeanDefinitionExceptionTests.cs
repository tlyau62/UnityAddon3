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

namespace UnityAddon.CoreTest.DependencyExceptions.NoUniqueBeanDefinition.NoUniqueBeanDefinitionExceptionTests
{
    public interface IB { }

    [Component]
    public class B1 : IB { }

    [Component]
    public class B2 : IB { }

    [Component]
    public class PropService
    {
        [Dependency]
        public IB B { get; set; }
    }

    [Component]
    public class CtorService
    {
        public CtorService(IB B)
        {
        }
    }

    [Trait("DependencyExceptions", "NoUniqueBeanDefinition")]
    public class NoUniqueBeanDefinitionExceptionTests
    {
        [Fact]
        public void PropertyFill_NoUniqueBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var ex = Assert.Throws<BeanCreationException>(() => appContext.Resolve<PropService>());

            Assert.Equal($"Property B in {typeof(PropService).FullName} required a single bean, but 2 were found:\r\n" +
                $"- {typeof(B1).Name}: defined in namespace [{typeof(B1).Namespace}]\r\n" +
                $"- {typeof(B2).Name}: defined in namespace [{typeof(B2).Namespace}]", ex.Message);
        }

        [Fact]
        public void ParameterFill_NoUniqueBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var ex = Assert.Throws<BeanCreationException>(() => appContext.Resolve<CtorService>());

            Assert.Equal($"Parameter 0 of Constructor in {typeof(CtorService).FullName} required a single bean, but 2 were found:\r\n" +
                $"- {typeof(B1).Name}: defined in namespace [{typeof(B1).Namespace}]\r\n" +
                $"- {typeof(B2).Name}: defined in namespace [{typeof(B2).Namespace}]", ex.Message);
        }
    }

}
