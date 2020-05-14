using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Core.Reflection
{
    /// <summary>
    /// For querying any methods in a type
    /// </summary>
    public static class MethodSelector
    {
        public static IEnumerable<MethodInfo> GetAllMethodsByAttribute<TAttribute>(Type type, bool isInherited = false) where TAttribute : Attribute
        {
            return GetAllMethods(type).Where(m => m.HasAttribute<TAttribute>(isInherited));
        }

        public static IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }
    }
}
