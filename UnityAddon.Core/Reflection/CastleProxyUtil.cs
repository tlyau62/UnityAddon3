using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UnityAddon.Core.Reflection
{
    public static class CastleProxyUtil
    {
        public static void MergeProxy(object proxy, IEnumerable<IInterceptor> interceptors)
        {
            dynamic dyn = proxy;
            List<IInterceptor> itors = dyn.__interceptors;

            itors.AddRange(interceptors);
        }
    }
}
