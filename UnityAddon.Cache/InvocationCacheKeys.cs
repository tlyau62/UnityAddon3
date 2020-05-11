using System.Collections.Concurrent;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Cache
{
    [Component]
    public class InvocationCacheKeys : ConcurrentBag<InvocationCacheKey>
    { }
}
