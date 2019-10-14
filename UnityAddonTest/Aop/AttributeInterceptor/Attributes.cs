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
            return new IncInterceptor(containerRegistry.Resolve<Counter>());
        }

        private class IncInterceptor : IInterceptor
        {
            private Counter _counter;

            public IncInterceptor(Counter counter)
            {
                _counter = counter;
            }

            public void Intercept(IInvocation invocation)
            {
                _counter.Count++;
                invocation.Proceed();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Mul2Attribute : AopInterceptorAttribute
    {
        public override IInterceptor CreateInterceptor(IContainerRegistry containerRegistry)
        {
            return new Mul2Interceptor(containerRegistry.Resolve<Counter>());
        }

        private class Mul2Interceptor : IInterceptor
        {
            private Counter _counter;

            public Mul2Interceptor(Counter counter)
            {
                _counter = counter;
            }

            public void Intercept(IInvocation invocation)
            {
                _counter.Count *= 2;
                invocation.Proceed();
            }
        }
    }
}
