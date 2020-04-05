using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Aop
{
    /// <summary>
    /// Hold all the aop interceptors scanned after component scanning.
    /// </summary>
    public class AopInterceptorContainer
    {
        private readonly IDictionary<Type, IEnumerable<IInterceptor>> _interceptorMap;

        public AopInterceptorContainer(IDictionary<Type, IEnumerable<IInterceptor>> interceptorMap)
        {
            _interceptorMap = interceptorMap;
        }

        /// <summary>
        /// Use AttributeTargets.All to get all interceptors
        /// </summary>
        public IDictionary<Type, IEnumerable<IInterceptor>> FindInterceptors(AttributeTargets interceptorType)
        {
            return _interceptorMap.Where(entry => IsAttributeTargetMatch(interceptorType, entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private bool IsAttributeTargetMatch(AttributeTargets requiredInterceptorType, AttributeTargets actualInterceptorType)
        {
            return (actualInterceptorType & requiredInterceptorType) == requiredInterceptorType;
        }
    }
}
