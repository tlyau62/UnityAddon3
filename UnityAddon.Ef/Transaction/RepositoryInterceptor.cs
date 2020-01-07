using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Ef.Transaction
{
    /// <summary>
    /// Auto open a db connection for any bean marked with Repository attribute
    /// </summary>
    [Component]
    public class RepositoryInterceptor : IAttributeInterceptor<RepositoryAttribute>
    {
        [Dependency]
        public DataSourceExtractor DataSourceExtractor { get; set; }

        [Dependency]
        public RequireDbContextHandler RequireDbContextHandler { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var dataSource = DataSourceExtractor.ExtractDataSource(invocation.TargetType);

            RequireDbContextHandler.InvokeContextHandler(dataSource, invocation, false);
        }

    }
}
