using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;
using UnityAddon.Aop;
using UnityAddon.Attributes;
using UnityAddon.Reflection;

namespace UnityAddon.EF.Transaction
{
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
