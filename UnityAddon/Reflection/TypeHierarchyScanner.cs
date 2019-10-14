﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Attributes;

namespace UnityAddon.Reflection
{
    public static class TypeHierarchyScanner
    {
        public static IEnumerable<Type> GetAssignableTypes(Type type)
        {
            var types = new List<Type> { type };

            types.AddRange(type.GetInterfaces());

            while (type.BaseType != null && type.BaseType != typeof(object))
            {
                types.Add(type.BaseType);
                type = type.BaseType;
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
            return types.Select(t => Type.GetType($"{t.Namespace}.{t.Name}, {t.Assembly.FullName}"));
        }
    }
}
