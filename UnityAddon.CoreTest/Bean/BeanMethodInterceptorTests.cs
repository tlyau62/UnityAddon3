using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using UnityAddon.Test;
using UnityAddon.Test.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.Bean
{
    [Configuration]
    public class Config
    {
        [Bean]
        [Scope(ScopeType.Transient)]
        public virtual string TestBean()
        {
            return "test";
        }
    }

    [ComponentScan]
    public class BeanMethodInterceptorTests : UnityAddonTest
    {
        public BeanMethodInterceptorTests(UnityAddonTestFixture testFixture) : base(testFixture)
        {
        }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        public void Intercept(int size)
        {
            var tasks = new Task[size];

            for (var i = 0; i < size; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => Assert.Equal("test", Sp.GetRequiredService<string>())); // Invocation must be null
            }

            Task.WaitAll(tasks);
        }
    }
}
