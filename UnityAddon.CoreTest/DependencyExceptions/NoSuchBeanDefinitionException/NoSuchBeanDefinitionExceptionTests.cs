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

namespace UnityAddon.CoreTest.DependencyExceptions.NoUniqueBeanDefinition.NoSuchBeanDefinitionExceptionTests
{
    public interface IB { }

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

    [Trait("DependencyExceptions", "NoSuchBeanDefinition")]
    public class NoSuchBeanDefinitionExceptionTests
    {
        [Fact]
        public void PropertyFill_NoSuchBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => appContext.Resolve<PropService>());

            Assert.Equal($"Property B in {typeof(PropService).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }

        [Fact]
        public void ParamterFill_NoSuchBeanDefinition_ExceptionThrown()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, false, GetType().Namespace);

            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => appContext.Resolve<CtorService>());

            Assert.Equal($"Parameter 0 of Constructor in {typeof(CtorService).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }
    }

}
