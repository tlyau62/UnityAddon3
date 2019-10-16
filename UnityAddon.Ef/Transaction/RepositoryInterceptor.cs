using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.EF.Transaction
{
    [Component]
    public class RepositoryInterceptor : IAttributeInterceptor<RepositoryAttribute>
    {
        [Dependency]
        public IRequireDbContextHandler RequireDbContextHandler { get; set; }

        public void Intercept(IInvocation invocation)
        {
            RequireDbContextHandler.DoInDbContext(invocation, false);
        }
    }
}
