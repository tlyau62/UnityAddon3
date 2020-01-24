using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Bean
{
    public static class Ordered
    {
        public const int LOWEST_PRECEDENCE = int.MaxValue;

        public const int HIGHEST_PRECEDENCE = int.MinValue;

        public static int GetOrder(this Type type)
        {
            if (type.HasAttribute<OrderAttribute>())
            {
                return type.GetAttribute<OrderAttribute>().Order;
            }

            return LOWEST_PRECEDENCE;
        }
    }
}
