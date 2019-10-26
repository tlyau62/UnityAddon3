﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Reflection
{
    /// <summary>
    /// Scan for type hierarchy.
    /// </summary>
    public static class TypeHierarchyScanner
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

            return LoadAllTypes(types);
        }

        public static IEnumerable<Type> GetInterfaces(Type type)
        {
            return LoadAllTypes(type.GetInterfaces());
        }

        /// <summary>
        /// Garantee all type objects are pointed to the distinct type object.
        /// It is possible that 2 types object of a same type are different.
        /// </summary>
        public static IEnumerable<Type> LoadAllTypes(IEnumerable<Type> types)
        {
            return types.Select(t =>
            {
                if (!t.IsGenericType || !t.ContainsGenericParameters)
                {
                    return LoadType(t);
                }

                return t;
            });
        }

        public static Type LoadType(Type type)
        {
            var loadedType = Type.GetType(TypeToString(type));

            return loadedType ?? type; // proxy type will return null
        }

        private static string TypeToString(Type t)
        {
            var str = $"{t.Namespace}.{t.Name}{TypeArgs(t)}, {t.Assembly.FullName}";

            return str;
        }

        private static string TypeArgs(Type t)
        {
            if (t.GetGenericArguments().Length == 0)
            {
                return "";
            }

            var str = "[[" + string.Join(',', t.GetGenericArguments().Select(arg => TypeToString(arg))) + "]]";

            return str;
        }
    }
}
