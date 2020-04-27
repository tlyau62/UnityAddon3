using System;
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
    [Scope(ScopeType.Singleton)]
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
    public class SingletonBeanTests : UnityAddonDefaultTest
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        //public SingletonBeanTests() : base(true)
        //{
        //}

        [Fact]
        public void ApplicationContext_PreinstantiateSingletonBean_SingletonBeanCreatedAtApplicationStart()
        {
            Container.Resolve<SingletonService>();
            Assert.Equal(1, Container.Resolve<Counter>().Count);
        }
    }
}
