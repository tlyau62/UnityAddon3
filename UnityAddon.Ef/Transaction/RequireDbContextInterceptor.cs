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
        [Dependency]
        public DataSourceExtractor DataSourceExtractor { get; set; }

        [Dependency]
        public RequireDbContextHandler RequireDbContextHandler { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var dataSource = DataSourceExtractor.ExtractDataSource(invocation.MethodInvocationTarget);
            var tx = invocation.MethodInvocationTarget.GetAttribute<RequireDbContextAttribute>().Transactional;

            RequireDbContextHandler.InvokeContextHandler(dataSource, invocation, tx);
        }

    }
}
