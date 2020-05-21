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

namespace UnityAddon.CoreTest.DependencyExceptions.NoUniqueBeanDefinition.Property.NoSuchBeanDefinitionExceptionTests
{
    public interface IB { }

    [Component]
    public class PropService
    {
        [Dependency]
        public IB B { get; set; }
    }

    [ComponentScan]
    public class NoSuchBeanDefinitionPropertyExceptionTests : UnityAddonTest
    {
        public NoSuchBeanDefinitionPropertyExceptionTests(UnityAddonTestFixture testFixture) : base(testFixture, true)
        {
        }

        [Fact]
        public void Properties()
        {
            var ex = Assert.Throws<NoSuchBeanDefinitionException>(() => Refresh());

            Assert.Equal($"Property B in {typeof(PropService).FullName} required a bean of type '{typeof(IB).FullName}' that could not be found.", ex.Message);
        }
    }

}
