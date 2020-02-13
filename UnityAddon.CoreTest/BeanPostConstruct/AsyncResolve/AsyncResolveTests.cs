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
        public IUnityContainer UnityContainer { get; set; }

        [PostConstruct]
        public void Init()
        {
            var tasks = new List<Task>();

            tasks.Add(Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    UnityContainer.Resolve<AsyncRepoB>();
                }
            }));

            for (var i = 0; i < 100; i++)
            {
                UnityContainer.Resolve<AsyncRepoA>();
            }
        }
    }

    [Trait("BeanPostConstruct", "AsyncResolve")]
    public class AsyncResolveTests
    {
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
        [InlineData(1)]
        [InlineData(10)]
        public void BeanDependencyValidatorStrategy_AsyncResolveBean_AllBeanResolved(int loop)
        {
            for (var i = 0; i < loop; i++)
            {
                var appCtx = new ApplicationContext(new UnityContainer(), GetType().Namespace);
            }
        }
    }
}
