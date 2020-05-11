using System;

namespace UnityAddon.Cache
{
    /*
     * return cached result if the method has been called with the same args before
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class CachableAttribute : Attribute
    {
    }
}
