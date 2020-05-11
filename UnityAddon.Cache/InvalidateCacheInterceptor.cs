using Castle.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Cache
{
    [Component]
    [AopAttribute(typeof(InvalidateCacheAttribute))]
    public class InvalidateCacheInterceptor : IInterceptor
    {
        [Dependency]
        public IMemoryCache MemoryCache { get; set; }

        [Dependency]
        public InvocationCacheKeys InvocationCacheKeys { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.MethodInvocationTarget.GetCustomAttribute<InvalidateCacheAttribute>();
            InvocationCacheKeys.Where(k => attr.AffectedTypes.Contains(k.Method.DeclaringType))
                .ToList()
                .ForEach(k =>
                {
                    MemoryCache.Remove(k);
                    InvocationCacheKeys.TryTake(out k);
                });

            invocation.Proceed();
        }
    }
}
