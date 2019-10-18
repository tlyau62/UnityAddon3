﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean.SingletonBean
{
    [Component]
    public class Counter
    {
        public int Count { get; set; } = 0;
    }

    [Component]
    [Scope(ScopeType.ContainerControlled)]
    public class SingletonService
    {
        [Dependency]
        public Counter Counter { get; set; }

        [PostConstruct]
        public void Init()
        {
            Counter.Count++;
        }
    }

    [Component]
    [Scope(ScopeType.Transient)]
    public class TransientService
    {
        [Dependency]
        public Counter Counter { get; set; }

        [PostConstruct]
        public void Init()
        {
            Counter.Count += 2;
        }
    }

    [Trait("Bean", "SingletonBean")]
    public class SingletonBeanTests
    {
        [Fact]
        public void ApplicationContext_PreinstantiateSingletonBean_SingletonBeanCreatedAtApplicationStart()
        {
            var container = new UnityContainer();
            var appContext = new ApplicationContext(container, GetType().Namespace);

            var counter = appContext.Resolve<Counter>();

            Assert.Equal(1, counter.Count);
        }
    }
}