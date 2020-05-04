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
    public class AopInterceptorContainerBuilder
    {
        private readonly IDictionary<Type, IList<Type>> _interceptorMap; // (attrtype, interceptor type)

        public AopInterceptorContainerBuilder()
        {
            _interceptorMap = new Dictionary<Type, IList<Type>>();
        }

        public AopInterceptorContainer Build(IServiceProvider sp)
        {
            return new AopInterceptorContainer(_interceptorMap
                .ToDictionary(
                    e => e.Key,
                    e => e.Value.Select(t => (IInterceptor)sp.GetRequiredService(t))));
        }

        public AopInterceptorContainerBuilder AddAopIntercetor<TAopAttribute, TInterceptor>()
            where TAopAttribute : Attribute
            where TInterceptor : IInterceptor
        {
            AddAopIntercetor(typeof(TAopAttribute), typeof(TInterceptor));

            return this;
        }

        public AopInterceptorContainerBuilder AddAopIntercetor<TInterceptor>()
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
            if (_interceptorMap.ContainsKey(attrType))
            {
                _interceptorMap[attrType].Add(interceptorType);
            }
            else
            {
                _interceptorMap[attrType] = new List<Type>() { interceptorType };
            }
        }

    }

}
