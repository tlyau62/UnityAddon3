using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Xunit;

namespace UnityAddon.CoreTest.BeanPostConstruct.AsyncResolve
{
    [Component]
    [Scope(ScopeType.Transient)]
    public class AsyncRepoA { }

    [Component]
    [Scope(ScopeType.Transient)]
    public class AsyncRepoB { }

    public interface IAsyncService { }

    [Component]
    public class AsyncService : IAsyncService
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [PostConstruct]
        public void Init()
        {
            Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    Sp.GetRequiredService<AsyncRepoB>();
                }
            });

            for (var i = 0; i < 100; i++)
            {
                Sp.GetRequiredService<AsyncRepoA>();
            }
        }
    }

    [ComponentScan(typeof(AsyncResolveTests))]
    public class AsyncResolveTests : UnityAddonTest
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        /// <summary>
        /// Sequential
        /// Time   Description
        /// 0      Resolve A (push A)
        /// 1      Finish Resolve A (pop A)
        /// 2      Resolve B (pop B)
        /// 3      Finish Resolve A (pop B)
        /// 
        /// Async (stack is shared)
        /// 0      Resolve A (push A)
        /// 1      Resolve B (push B)
        /// 2      Finish Resolve A (pop A, B)
        /// 3      Finish Resolve B (stack empty)
        /// 
        /// If use IAsyncLocalFactory<Stack<ResolveStackEntry>> in class BeanDependencyValidatorStrategy,
        /// it will cause stack empty exception.
        /// </summary>
        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        public void AsyncResolve(int loop)
        {
            for (var i = 0; i < loop; i++)
            {
                Sp.GetRequiredService<IAsyncService>();
            }
        }
    }
}
