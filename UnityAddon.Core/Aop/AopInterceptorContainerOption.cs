using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Core.Aop
{
    public class AopInterceptorContainerOption
    {
        public IDictionary<Type, IList<Type>> InterceptorMap { get; } // (attrtype, interceptor type)

        public AopInterceptorContainerOption()
        {
            InterceptorMap = new Dictionary<Type, IList<Type>>();
        }

        public AopInterceptorContainerOption AddAopIntercetor<TAopAttribute, TInterceptor>()
            where TAopAttribute : Attribute
            where TInterceptor : IInterceptor
        {
            AddAopIntercetor(typeof(TAopAttribute), typeof(TInterceptor));

            return this;
        }

        public AopInterceptorContainerOption AddAopIntercetor<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            var aopAttributes = typeof(TInterceptor).GetAllAttributes<AopAttributeAttribute>();

            foreach (var aopAttr in aopAttributes)
            {
                foreach (var attr in aopAttr.AopAttributes)
                {
                    AddAopIntercetor(attr, typeof(TInterceptor));
                }
            }

            return this;
        }

        private void AddAopIntercetor(Type attrType, Type interceptorType)
        {
            if (InterceptorMap.ContainsKey(attrType))
            {
                InterceptorMap[attrType].Add(interceptorType);
            }
            else
            {
                InterceptorMap[attrType] = new List<Type>() { interceptorType };
            }
        }

    }

}
