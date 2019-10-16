using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Aop;
using UnityAddon.Attributes;

namespace UnityAddonTest.Aop.AttributeInterceptor
{
    [Component]
    public class IncInterceptor : IAttributeInterceptor<IncAttribute>
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            Counter.Count++;
            invocation.Proceed();
        }
    }

    [Component]
    public class Mul2Interceptor : IAttributeInterceptor<Mul2Attribute>
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            Counter.Count *= 2;
            invocation.Proceed();
        }
    }
}
