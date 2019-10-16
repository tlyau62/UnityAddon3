using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.CoreTest.Aop.MethodAttributeInterceptor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IncAttribute : Attribute
    {
        public int Value { get; set; }

        public IncAttribute(int value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MulAttribute : Attribute
    {
        public int Value { get; set; }

        public MulAttribute(int value)
        {
            Value = value;
        }
    }
}
