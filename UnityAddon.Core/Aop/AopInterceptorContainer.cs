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
    public class AopInterceptorContainer
    {
        [Dependency]
        public AopInterceptorContainerOption AopInterceptorContainerOption { get; set; }

        [Dependency]
        public ApplicationContext ApplicationContext { get; set; }

        [Dependency]
        public IServiceProvider Sp { get; set; }

        private IDictionary<Type, IEnumerable<IInterceptor>> _interceptorMap;

        private bool _isInit = false;

        public void Init()
        {
            if (_isInit)
            {
                throw new InvalidOperationException("Can only initialized once");
            }

            ApplicationContext.AddContextEntry(entry =>
            {
                entry.ConfigureBeanDefinitions(defs =>
                {
                    foreach (var type in AopInterceptorContainerOption.InterceptorMap.Values.SelectMany(v => v))
                    {
                        defs.Add(new TypeBeanDefintion(type, type, null, ScopeType.Singleton));
                    }
                });
            });

            ApplicationContext.Refresh();

            _interceptorMap = AopInterceptorContainerOption.InterceptorMap.ToDictionary(
                e => e.Key,
                e => e.Value.Select(t => (IInterceptor)Sp.GetRequiredService(t)));

            _isInit = true;
        }

        /// <summary>
        /// Use AttributeTargets.All to get all interceptors
        /// </summary>
        public IDictionary<Type, IEnumerable<IInterceptor>> FindInterceptors(AttributeTargets interceptorType)
        {
            if (!_isInit)
            {
                throw new InvalidOperationException("AopInterceptorContainer is not initialized");
            }

            return _interceptorMap.Where(entry => IsAttributeTargetMatch(interceptorType, entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private bool IsAttributeTargetMatch(AttributeTargets requiredInterceptorType, AttributeTargets actualInterceptorType)
        {
            return (actualInterceptorType & requiredInterceptorType) > 0;
        }
    }
}
