using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Cache
{
    [Component]
    [AopAttribute(typeof(CachableAttribute))]
    public class CacheInterceptor : IInterceptor
    {
        [Dependency]
        public IMemoryCache MemoryCache { get; set; }

        [Dependency]
        public InvocationCacheKeys InvocationCacheKeys { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var key = new InvocationCacheKey { Method = invocation.MethodInvocationTarget, Arguments = invocation.Arguments };

            invocation.ReturnValue = MemoryCache.GetOrCreate(key, e =>
            {
                InvocationCacheKeys.Add(key);
                invocation.Proceed();
                return invocation.ReturnValue;
            });
        }
    }
}
