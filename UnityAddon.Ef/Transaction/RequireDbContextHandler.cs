using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using Unity;
using UnityAddon.Core;
using UnityAddon.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Ef.Transaction
{
    [Component]
    public class RequireDbContextHandler
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        public void InvokeContextHandler(Type dataSource, IInvocation invocation, bool transactional)
        {
            dynamic txMan = Sp.GetRequiredService(typeof(ITransactionManager<>).MakeGenericType(dataSource));

            txMan.DoInDbContext(invocation, transactional);
        }
    }
}
