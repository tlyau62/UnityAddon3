using System;
using System.Linq;
using System.Reflection;

namespace UnityAddon.Cache
{
    public class InvocationCacheKey
    {
        public MethodInfo Method { get; set; }
        public object[] Arguments { get; set; }

        public override bool Equals(object obj)
        {
            return obj is InvocationCacheKey key &&
                   Method.Equals(key.Method) &&
                   Enumerable.SequenceEqual(Arguments, key.Arguments);
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(Method);
            Arguments.ToList()
                .ForEach(hashcode.Add);
            return hashcode.ToHashCode();
        }
    }
}
