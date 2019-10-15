using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Aop
{
    public interface IInterceptorFactory<TAttribute> where TAttribute : Attribute
    {
        IInterceptor CreateInterceptor();
    }
}
