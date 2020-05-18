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
    public class AopInterceptorOption
    {
        private IDictionary<Type, IList<Type>> _interceptorMap; // (attrtype, interceptor type)

        public IDictionary<Type, IEnumerable<Type>> InterceptorMap => _interceptorMap.ToDictionary(e => e.Key, e => (IEnumerable<Type>)e.Value);

        public AopInterceptorOption()
        {
            _interceptorMap = new Dictionary<Type, IList<Type>>();
        }

        public AopInterceptorOption AddAopIntercetor<TAopAttribute, TInterceptor>()
            where TAopAttribute : Attribute
            where TInterceptor : IInterceptor
        {
            AddAopIntercetor(typeof(TAopAttribute), typeof(TInterceptor));

            return this;
        }

        public AopInterceptorOption AddAopIntercetor<TInterceptor>()
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
