using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Ef.Transaction
{
    /// <summary>
    /// Handle transactional logic.
    /// </summary>
    [Component]
    public class RequireDbContextInterceptor : IAttributeInterceptor<RequireDbContextAttribute>
    {
        private static readonly MethodInfo DoInDbContextInvokerMethod = typeof(RequireDbContextInterceptor).GetMethod(nameof(DoInDbContextInvoker), BindingFlags.NonPublic | BindingFlags.Instance);

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public DataSourceExtractor DataSourceExtractor { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var dataSource = DataSourceExtractor.ExtractDataSource(invocation.MethodInvocationTarget);

            try
            {
                DoInDbContextInvokerMethod
                    .MakeGenericMethod(dataSource)
                    .Invoke(this, new object[] { invocation, invocation.MethodInvocationTarget.GetAttribute<RequireDbContextAttribute>().Transactional });
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        private void DoInDbContextInvoker<T>(IInvocation invocation, bool transactional)
        {
            var requireDbContextHandler = ContainerRegistry.Resolve<IRequireDbContextHandler<T>>();

            requireDbContextHandler.DoInDbContext(invocation, transactional);
        }
    }
}
