using System;

namespace UnityAddon.Cache
{
    /*
     * calling this method will invalidate cache of all methods in affected types
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class InvalidateCacheAttribute : Attribute
    {
        public Type[] AffectedTypes { get; }

        public InvalidateCacheAttribute(params Type[] affectedTypes) {
            AffectedTypes = affectedTypes;
        }
    }
}
