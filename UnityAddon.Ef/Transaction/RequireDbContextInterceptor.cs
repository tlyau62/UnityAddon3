using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.EF.Transaction
{
    /// <summary>
    /// Handle transactional logic.
    /// </summary>
    [Component]
    public class RequireDbContextInterceptor : IAttributeInterceptor<RequireDbContextAttribute>
    {
        [Dependency]
        public IRequireDbContextHandler RequireDbContextHandler { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var tx = invocation.MethodInvocationTarget.GetAttribute<RequireDbContextAttribute>().Transactional;

            RequireDbContextHandler.DoInDbContext(invocation, tx);
        }
    }
}
