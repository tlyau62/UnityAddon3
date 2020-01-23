using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PrefixAttribute : Attribute
    {
        public string Value { get; set; }

        public PrefixAttribute(string value)
        {
            Value = value;
        }
    }
}
