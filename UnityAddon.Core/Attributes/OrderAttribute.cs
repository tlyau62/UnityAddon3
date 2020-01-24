﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}
