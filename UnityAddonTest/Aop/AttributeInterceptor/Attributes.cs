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
    public class IncAttribute : AopInterceptorAttribute
    {
        public override IInterceptor CreateInterceptor(IContainerRegistry containerRegistry)
        {
            return containerRegistry.Resolve<IncInterceptor>();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Mul2Attribute : AopInterceptorAttribute
    {
        public override IInterceptor CreateInterceptor(IContainerRegistry containerRegistry)
        {
            return containerRegistry.Resolve<Mul2Interceptor>();
        }
    }
}
