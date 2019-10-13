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
        public static bool HasAttribute(this MemberInfo memberInfo, Type attribute, bool isInherited = false)
        {
            return memberInfo.GetAllAttributes(attribute, isInherited).Count() > 0;
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return HasAttribute(memberInfo, typeof(TAttribute), isInherited);
        }

        public static Attribute GetAttribute(this MemberInfo memberInfo, Type attribute, bool isInherited = false)
        {
            var attrs = memberInfo.GetAllAttributes(attribute, isInherited);

            if (attrs.Count() > 1)
            {
                throw new InvalidOperationException($"No single attribute {attribute} is found.");
            }

            return attrs.SingleOrDefault();
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return (TAttribute)GetAttribute(memberInfo, typeof(TAttribute), isInherited);
        }

        public static IEnumerable<Attribute> GetAllAttributes(this MemberInfo memberInfo, Type attribute, bool isInherited = false)
        {
            var attrs = memberInfo.GetCustomAttributes(attribute, false).Cast<Attribute>();

            return isInherited ? attrs : attrs.Where(attr => attr.GetType() == attribute);
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this MemberInfo memberInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return GetAllAttributes(memberInfo, typeof(TAttribute), isInherited).Cast<TAttribute>();
        }



        public static bool HasAttribute(this ParameterInfo paramInfo, Type attribute, bool isInherited = false)
        {
            return paramInfo.GetAllAttributes(attribute, isInherited).Count() > 0;
        }

        public static bool HasAttribute<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return HasAttribute(paramInfo, typeof(TAttribute), isInherited);
        }

        public static Attribute GetAttribute(this ParameterInfo paramInfo, Type attribute, bool isInherited = false)
        {
            var attrs = paramInfo.GetAllAttributes(attribute, isInherited);

            if (attrs.Count() > 1)
            {
                throw new InvalidOperationException($"No single attribute {attribute} is found.");
            }

            return attrs.SingleOrDefault();
        }

        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return (TAttribute)GetAttribute(paramInfo, typeof(TAttribute), isInherited);
        }

        public static IEnumerable<TAttribute> GetAllAttributes<TAttribute>(this ParameterInfo paramInfo, bool isInherited = false) where TAttribute : Attribute
        {
            return GetAllAttributes(paramInfo, typeof(TAttribute), isInherited).Cast<TAttribute>();
        }

        public static IEnumerable<Attribute> GetAllAttributes(this ParameterInfo paramInfo, Type attribute, bool isInherited = false)
        {
            var attrs = paramInfo.GetCustomAttributes(attribute, false).Cast<Attribute>();

            return isInherited ? attrs : attrs.Where(attr => attr.GetType() == attribute);
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
