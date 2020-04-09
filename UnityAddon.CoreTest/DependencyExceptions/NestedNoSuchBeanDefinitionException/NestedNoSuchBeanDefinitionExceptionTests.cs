﻿using System;
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

namespace UnityAddon.CoreTest.DependencyExceptions.NestedNoSuchBeanDefinitionException.NestedNoSuchBeanDefinitionExceptionTests
{
    public interface IRoot { }

    public interface ICommonService { }

    public interface IService { }

    [Component]
    public class CommonService : ICommonService
    {
        [Dependency]
        public IRoot Root { get; set; }
    }

    [Component]
    public class Service : IService
    {
        [Dependency]
        public ICommonService CommonService { get; set; }
    }

    [Trait("DependencyExceptions", "NoSuchBeanDefinition")]
    public class NestedNoSuchBeanDefinitionExceptionTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Fact]
        public void PropertyFill_NestedNoSuchBeanDefinition_ExceptionThrown()
        {
            var ex = Assert.Throws<BeanCreationException>(() => UnityContainer.Resolve<IService>());

            Assert.Equal($"Property Root in {typeof(CommonService).FullName} required a bean of type '{typeof(IRoot).FullName}' that could not be found.", ex.Message);
        }
    }

}
