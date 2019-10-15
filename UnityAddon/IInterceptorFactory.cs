using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon
{
    public interface IInterceptorFactory<TAttribute> where TAttribute : Attribute
    {
        IInterceptor CreateInterceptor();
    }
}
