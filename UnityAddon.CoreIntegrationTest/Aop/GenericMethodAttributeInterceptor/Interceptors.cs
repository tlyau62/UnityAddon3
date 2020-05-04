using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.CoreTest.Aop.GenericMethodAttributeInterceptor
{
    [AopAttribute(typeof(PrefixAttribute))]
    public class IncInterceptor : IInterceptor
    {
        [Dependency]
        public Logger Logger { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.MethodInvocationTarget.GetAttribute<PrefixAttribute>();

            Logger.Log += attr.Value;

            invocation.Proceed();
        }
    }
}
