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
using UnityAddon.Core.Bean.Config;

namespace UnityAddon.Core.Aop
{
    /// <summary>
    /// Hold all the aop interceptors scanned after component scanning.
    /// </summary>
    public class AopInterceptorContainer : IContextPostRegistryInitiable
    {
        [Dependency]
        public IServicePostRegistry ServicePostRegistry { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        private IDictionary<Type, IEnumerable<IInterceptor>> _interceptorMap = new Dictionary<Type, IEnumerable<IInterceptor>>();

        public void Initialize()
        {
            var interceptorMaps = Sp.GetServices<AopInterceptorOption>()
                .Select(option => option.InterceptorMap);
            IDictionary<Type, IList<Type>> interceptorMap = new Dictionary<Type, IList<Type>>();

            if (interceptorMaps.Count() > 0)
            {
                interceptorMap = interceptorMaps.Aggregate((a, map) =>
                {
                    foreach (var entry in map)
                    {
                        a.Add(entry.Key, entry.Value);
                    }

                    return a;
                });
            }

            foreach (var entry in interceptorMap)
            {
                _interceptorMap[entry.Key] = entry.Value.Select(t =>
                {
                    if (!Sp.IsRegistered(t))
                    {
                        ServicePostRegistry.AddSingleton(t, t);
                    }

                    return (IInterceptor)Sp.GetRequiredService(t);
                });
            }
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
            return (actualInterceptorType & requiredInterceptorType) > 0;
        }

    }
}
