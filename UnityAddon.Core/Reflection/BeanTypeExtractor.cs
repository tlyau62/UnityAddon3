using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Reflection
{
    public static class BeanTypeExtractor
    {
        public static Type ExtractBeanType(Type type)
        {
            if (type.IsGenericType && type.ContainsGenericParameters)
            {
                return type.GetGenericTypeDefinition();

            }

            return type;
        }
    }
}
