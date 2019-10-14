﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.BeanBuildStrategies;
using UnityAddon.Reflection;

namespace UnityAddon
{
    [Component]
    public class AopInterceptor : IInterceptor
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public InterfaceProxyFactory InterfaceProxyFactory { get; set; }

        public void Intercept(IInvocation invocation)
        {
            object newProxy;
            MethodInfo proxyMethod;

            var aopInterceptors = GetMethodInterceptors(invocation.MethodInvocationTarget);
            var remainingInterceptors = GetRemainingInterceptors(invocation.Proxy);
            var mergeInterceptors = aopInterceptors.Union(remainingInterceptors);

            newProxy = InterfaceProxyFactory.CreateInterfaceProxy(invocation.InvocationTarget, mergeInterceptors.ToArray());
            proxyMethod = GetProxyMethod(invocation, newProxy);

            try
            {
                invocation.ReturnValue = proxyMethod.Invoke(newProxy, invocation.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw ex;
            }
        }

        private IEnumerable<IInterceptor> GetMethodInterceptors(MethodInfo method)
        {
            return AttributeExt.GetAllAttributes<AopInterceptorAttribute>(method, true)
                .Select(attr => attr.CreateInterceptor(ContainerRegistry));
        }

        /// <summary>
        /// Get all interceptors after the AopInterceptor from the proxy object.
        /// </summary>
        private IInterceptor[] GetRemainingInterceptors(object proxy)
        {
            var field = (FieldInfo)proxy.GetType().GetMember("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(0);
            var interceptors = (IInterceptor[])field.GetValue(proxy);
            var found = false;
            var extracts = new List<IInterceptor>();

            foreach (var interceptor in interceptors)
            {
                if (found)
                {
                    extracts.Add(interceptor);
                }
                else if (interceptor.GetType() == typeof(AopInterceptor))
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new InvalidOperationException("AopIntercetor is not found.");
            }

            return extracts.ToArray();
        }

        private MethodInfo GetProxyMethod(IInvocation invocation, object newProxy)
        {
            Type[] parameters;
            string[] methodFullname = invocation.ToString().Split('.');
            string methodName = methodFullname.Last().Replace('_', '.');
            Func<string, MethodInfo> getMethod;

            parameters = invocation.MethodInvocationTarget.GetParameters().Select(p => p.ParameterType).ToArray();
            getMethod = new Func<string, MethodInfo>(mName => newProxy.GetType().GetMethod(mName, parameters));

            return getMethod(methodName.Split('.')[1]) ?? getMethod(methodName); // "Method" : "IRepo.Method"
        }

    }
}
