using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Aop
{
    public interface IAttributeInterceptor<TAttribute> : IInterceptor where TAttribute : Attribute
    {
    }
}
