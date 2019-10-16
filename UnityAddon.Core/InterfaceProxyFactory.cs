using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core
{
    [Component]
    public class InterfaceProxyFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        public object CreateInterfaceProxy(object bean, params IInterceptor[] interceptors)
        {
            var itfs = TypeHierarchyScanner.GetInterfaces(bean.GetType());

            if (itfs.Count() == 0)
            {
                throw new InvalidOperationException("An aop proxy class must implement at least 1 interface.");
            }

            return ProxyGenerator.CreateInterfaceProxyWithTarget(
                itfs.First(),
                itfs.Skip(1).ToArray(),
                bean,
                interceptors);
        }
    }
}
