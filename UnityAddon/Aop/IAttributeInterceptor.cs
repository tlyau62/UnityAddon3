using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Aop
{
    public interface IAttributeInterceptor<TAttribute> : IInterceptor where TAttribute : Attribute
    {
    }
}
