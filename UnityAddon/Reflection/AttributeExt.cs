using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Reflection
{
    public static class AttributeExt
    {
        public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return memberInfo.GetAllAttributes<TAttribute>(isInherited).Count() > 0;
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = memberInfo.GetAllAttributes<TAttribute>(isInherited);

            if (attrs.Count() > 1)
            {
                throw new InvalidOperationException($"No single attribute {typeof(TAttribute).Name} is found.");
            }

            return attrs.SingleOrDefault();
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = memberInfo.GetCustomAttributes<TAttribute>(false);

            return isInherited ? attrs : attrs.Where(attr => attr.GetType() == typeof(TAttribute));
        }




        public static bool HasAttribute<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return paramInfo.GetAllAttributes<TAttribute>(isInherited).Count() > 0;
        }

        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = paramInfo.GetAllAttributes<TAttribute>(isInherited);

            if (attrs.Count() > 1)
            {
                throw new InvalidOperationException($"No single attribute {typeof(TAttribute).Name} is found.");
            }

            return attrs.SingleOrDefault();
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = paramInfo.GetCustomAttributes<TAttribute>(false);

            return isInherited ? attrs : attrs.Where(attr => attr.GetType() == typeof(TAttribute));
        }



        public static bool HasAttribute<TAttribute>(this Assembly asm, bool isInherited = false) where TAttribute : Attribute
        {
            return asm.GetAllAttributes<TAttribute>(isInherited).Count() > 0;
        }

        public static TAttribute GetAttribute<TAttribute>(this Assembly asm, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = asm.GetAllAttributes<TAttribute>(isInherited);

            if (attrs.Count() != 1)
            {
                throw new InvalidOperationException($"No single attribute {typeof(TAttribute).Name} is found.");
            }

            return attrs.Single();
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this Assembly asm, bool isInherited = false) where TAttribute : Attribute
        {
            var attrs = asm.GetCustomAttributes<TAttribute>();

            return isInherited ? attrs : attrs.Where(attr => attr.GetType() == typeof(TAttribute));
        }
    }
}
