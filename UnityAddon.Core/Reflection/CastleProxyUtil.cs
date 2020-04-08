using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityAddon.Core.Reflection
{
    public static class CastleProxyUtil
    {
        public static void MergeProxy(object proxy, IEnumerable<IInterceptor> interceptors)
        {
            var field = (FieldInfo)proxy.GetType().GetMember("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(0);
            var proxyInterceptors = (IInterceptor[])field.GetValue(proxy);

            field.SetValue(proxy, proxyInterceptors.ToList().Union(interceptors).ToArray());
        }
    }
}
