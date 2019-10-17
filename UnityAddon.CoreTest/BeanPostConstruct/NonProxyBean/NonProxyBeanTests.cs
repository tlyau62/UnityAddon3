﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.BeanPostConstruct.NonProxy
{
    [Component]
    public class StringStore
    {
        public string TestString { get; set; } = "";
    }

    [Component]
    public class ServiceA
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [PostConstruct]
        public void Init()
        {
            StringStore.TestString += "A";
        }
    }

    [Component]
    public class ServiceB
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [Dependency]
        public ServiceA ServiceA { get; set; }

        [PostConstruct]
        public void Init()
        {
            Assert.NotNull(ServiceA);
            StringStore.TestString += "B";
        }
    }

    [Component]
    public class ServiceC
    {
        [Dependency]
        public StringStore StringStore { get; set; }

        [Dependency]
        public ServiceB ServiceB { get; set; }

        [PostConstruct]
        public void Init()
        {
            Assert.NotNull(ServiceB);
            StringStore.TestString += "C";
        }
    }

    [Trait("BeanPostConstruct", "NonProxyBean")]
    public class NonProxyBeanTests
    {
        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(0, 2, 1)]
        [InlineData(1, 0, 2)]
        [InlineData(1, 2, 0)]
        [InlineData(2, 1, 0)]
        [InlineData(2, 0, 1)]
        public void BuildStrategy_PostConstructNonProxyBean_BeanPostConstructed(int orderA, int orderB, int orderC)
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var stringStore = appContext.Resolve<StringStore>();
            var resolveOrder = new Type[3];

            resolveOrder[orderA] = typeof(ServiceA);
            resolveOrder[orderB] = typeof(ServiceB);
            resolveOrder[orderC] = typeof(ServiceC);

            foreach (var t in resolveOrder)
            {
                appContext.Resolve(t);
            }

            Assert.Equal("ABC", stringStore.TestString);
        }
    }
}