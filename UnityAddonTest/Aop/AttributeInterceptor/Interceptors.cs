using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Attributes;

namespace UnityAddonTest.Aop.AttributeInterceptor
{
    [Component]
    public class IncInterceptor : IInterceptorFactory<IncAttribute>, IInterceptor
    {
        [Dependency]
        public Counter Counter { get; set; }

        public IInterceptor CreateInterceptor()
        {
            return this;
        }

        public void Intercept(IInvocation invocation)
        {
            Counter.Count++;
            invocation.Proceed();
        }
    }

    [Component]
    public class Mul2Interceptor : IInterceptorFactory<Mul2Attribute>, IInterceptor
    {
        [Dependency]
        public Counter Counter { get; set; }

        public IInterceptor CreateInterceptor()
        {
            return this;
        }

        public void Intercept(IInvocation invocation)
        {
            Counter.Count *= 2;
            invocation.Proceed();
        }
    }
}
