﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method)]
    public class ComponentScanAttribute : Attribute
    {
        public string[] BaseNamespaces { get; set; }
    }
}