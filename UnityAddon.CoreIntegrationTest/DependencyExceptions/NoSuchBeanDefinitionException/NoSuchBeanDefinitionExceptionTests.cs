using Microsoft.Extensions.DependencyInjection;
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
    public class NoSuchBeanDefinitionExceptionTests : DefaultTest
    {
        [Dependency]
        public IServiceProvider Sp { get; set; }

        [Fact]
        public void PropertyFill_NoSuchBeanDefinition_ExceptionThrown()
        {
            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => Sp.GetRequiredService<PropService>());

            Assert.Equal($"Property B in {typeof(PropService).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }

        [Fact]
        public void ParamterFill_NoSuchBeanDefinition_ExceptionThrown()
        {
            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => Sp.GetRequiredService<CtorService>());

            Assert.Equal($"Parameter 0 of Constructor in {typeof(CtorService).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }
    }

}
