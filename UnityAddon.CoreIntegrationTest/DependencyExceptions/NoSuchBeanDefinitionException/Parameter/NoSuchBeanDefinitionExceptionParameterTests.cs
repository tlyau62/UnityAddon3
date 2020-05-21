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
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;
using Assert = Xunit.Assert;

namespace UnityAddon.CoreTest.DependencyExceptions.NoUniqueBeanDefinition.Parameter.NoSuchBeanDefinitionExceptionParameterTests
{
    public interface IB { }

    [Component]
    public class CtorService
    {
        public CtorService(IB B)
        {
        }
    }

    [ComponentScan]
    public class NoSuchBeanDefinitionExceptionParameterTests : UnityAddonTest
    {
        public NoSuchBeanDefinitionExceptionParameterTests(UnityAddonTestFixture testFixture) : base(testFixture, true)
        {
        }

        [Fact]
        public void Paramters()
        {
            var ex = Assert.Throws<BeanCreationException>(() => Refresh());

            Assert.Equal($"Fail to satisfy any of these constructors\r\n- {typeof(CtorService).GetConstructors().First()}", ex.Message);
        }
    }

}
