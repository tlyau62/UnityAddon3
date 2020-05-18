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
using UnityAddon.Core.BeanDefinition.GeneralBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Core.Aop
{
    /// <summary>
    /// Hold all the aop interceptors scanned after component scanning.
    /// </summary>
    public class AopInterceptorContainer : IContextPostRegistryInitiable
    {
        [Dependency]
        public IServiceRegistry ServicePostRegistry { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        private readonly IDictionary<Type, IEnumerable<Type>> _interceptorMap = new Dictionary<Type, IEnumerable<Type>>();

        public void Initialize()
        {
            var regs = Sp.GetServices<AopInterceptorOption>().Select(option => option.InterceptorMap);

            foreach (var entry in Sp.GetServices<AopInterceptorOption>().SelectMany(option => option.InterceptorMap))
            {
                _interceptorMap.Add(entry.Key, entry.Value);
            }

            foreach (var t in _interceptorMap.Values.SelectMany(vals => vals).Distinct())
            {
                if (!Sp.IsRegistered(t))
                {
                    ServicePostRegistry.AddSingleton(t, t);
                }
            }
        }

        /// <summary>
        /// Use AttributeTargets.All to get all interceptors
        /// </summary>
        public IDictionary<Type, IEnumerable<Type>> FindInterceptors(AttributeTargets interceptorType)
        {
            return _interceptorMap.Where(entry => IsAttributeTargetMatch(interceptorType, entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private bool IsAttributeTargetMatch(AttributeTargets requiredInterceptorType, AttributeTargets actualInterceptorType)
        {
            return (actualInterceptorType & requiredInterceptorType) > 0;
        }

    }
}
