using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.Aop
{
    /// <summary>
    /// The interceptor used by any attribute.
    /// Any implemented class must be a component.
    /// </summary>
    public interface IAttributeInterceptor<TAttribute> : IInterceptor where TAttribute : Attribute
    {
    }
}
