using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Attributes;
using UnityAddon.Exceptions;
using UnityAddon.Reflection;

namespace UnityAddon.BeanBuildStrategies
{
    [Component]
    public class BeanInterceptionStrategy : BuilderStrategy
    {
        [Dependency]
        public AopInterceptor AopInterceptor { get; set; }

        [Dependency]
        public InterfaceProxyFactory InterfaceProxyFactory { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            var inteceptionMethods = MethodSelector.GetAllMethodsByAttribute<AopInterceptorAttribute>(context.Type, true);

            if (inteceptionMethods.Count() > 0)
            {
                context.Existing = InterfaceProxyFactory.CreateInterfaceProxy(context.Existing, AopInterceptor);
            }

            base.PostBuildUp(ref context);
        }

    }
}
