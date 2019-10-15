using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;

namespace UnityAddonTest.Aop.AttributeInterceptor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IncAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Mul2Attribute : Attribute
    {
    }
}
