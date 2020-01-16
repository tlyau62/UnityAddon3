using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Reflection
{
    /// <summary>
    /// Scan for type hierarchy.
    /// </summary>
    public static class TypeResolver
    {
        public static IEnumerable<Type> GetAssignableTypes(Type type)
        {
            var types = new List<Type> { type };

            if (!type.IsPrimitive && !type.IsEnum && type != typeof(string))
            {
                types.AddRange(type.GetInterfaces());

                while (type.BaseType != null && type.BaseType != typeof(object))
                {
                    types.Add(type.BaseType);
                    type = type.BaseType;
                }
            }

            return LoadTypes(types.ToArray());
        }

        public static IEnumerable<Type> GetInterfaces(Type type)
        {
            return LoadTypes(type.GetInterfaces());
        }

        /// <summary>
        /// Garantee all type objects are pointed to the distinct type object.
        /// It is possible that 2 types object of a same type are different.
        /// </summary>
        public static IEnumerable<Type> LoadTypes(params Type[] types)
        {
            return types.Select(type => LoadType(type));
        }

        public static Type LoadType(Type type)
        {
            var typeName = TypeToString(type);

            if (string.IsNullOrEmpty(typeName))
            {
                throw new InvalidOperationException("Type full name should not be null.");
            }

            return Type.GetType(typeName) ?? type; // proxy type maybe null
        }

        private static string TypeToString(Type t)
        {
            if (t.IsGenericTypeParameter)
            {
                return "";
            }

            var str = $"{t.Namespace}.{t.Name}{TypeArgs(t)}, {t.Assembly.FullName}";

            return str;
        }

        private static string TypeArgs(Type t)
        {
            if (t.GetGenericArguments().Length == 0)
            {
                return "";
            }

            var argParams = string.Join(',', t.GetGenericArguments().Select(arg => TypeToString(arg)));

            if (string.IsNullOrEmpty(argParams))
            {
                return argParams;
            }

            return "[[" + argParams + "]]";
        }
    }
}
