using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AopInterceptorAttribute : Attribute
    {
        public abstract IInterceptor CreateInterceptor(IContainerRegistry containerRegistry);
    }
}
