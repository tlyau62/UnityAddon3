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
        public static IEnumerable<MethodInfo> GetAllMethodsByAttribute<TAttribute>(Type type) where TAttribute : Attribute
        {
            return GetAllMethods(type).Where(m => m.GetCustomAttributes().Any(attr => attr is TAttribute));
        }

        public static IEnumerable<MethodInfo> GetAllMethods(Type type, BindingFlags flags = BindingFlags.Default)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | flags);
        }
    }
}
