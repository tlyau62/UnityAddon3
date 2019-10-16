﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon;
using UnityAddon.Aop;
using UnityAddon.Attributes;
using UnityAddon.Reflection;

namespace UnityAddonTest.Aop.MethodAttributeInterceptor
{
    [Component]
    public class IncInterceptor : IAttributeInterceptor<IncAttribute>
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.MethodInvocationTarget.GetAttribute<IncAttribute>();

            Counter.Count += attr.Value;
            invocation.Proceed();
        }
    }

    [Component]
    public class Mul2Interceptor : IAttributeInterceptor<MulAttribute>
    {
        [Dependency]
        public Counter Counter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.MethodInvocationTarget.GetAttribute<MulAttribute>();

            Counter.Count *= attr.Value;
            invocation.Proceed();
        }
    }
}
