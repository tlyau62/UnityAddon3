using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Reflection;

namespace UnityAddon.Aop
{
    public class InterceptorContainer
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public ContainerRegistry ContainerRegistry { get; set; }

        private IDictionary<Type, IList<IInterceptor>> _interceptorMap = new Dictionary<Type, IList<IInterceptor>>();

        private bool _isInitialized = false;

        /// <summary>
        /// Allow to build once only
        /// </summary>
        public void Build()
        {
            if (!BeanDefinitionContainer.HasBeanDefinition(typeof(IAttributeInterceptor<>)) || _isInitialized)
            {
                return;
            }

            foreach (var beanDef in BeanDefinitionContainer.GetAllBeanDefinitions(typeof(IAttributeInterceptor<>)))
            {
                var interceptorAttribute = GetAttributeType(beanDef.GetBeanType());
                IInterceptor interceptor = (IInterceptor)ContainerRegistry.Resolve(beanDef.GetBeanType(), beanDef.GetBeanName());

                if (_interceptorMap.ContainsKey(interceptorAttribute))
                {
                    _interceptorMap[interceptorAttribute].Add(interceptor);
                }
                else
                {
                    _interceptorMap[interceptorAttribute] = new List<IInterceptor>() { interceptor };
                }
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Use AttributeTargets.All to get all interceptors
        /// </summary>
        public IDictionary<Type, IList<IInterceptor>> FindInterceptors(AttributeTargets interceptorType)
        {
            return _interceptorMap.Where(entry => IsAttributeTargetMatch(interceptorType, entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private bool IsAttributeTargetMatch(AttributeTargets requiredInterceptorType, AttributeTargets actualInterceptorType)
        {
            return (actualInterceptorType & requiredInterceptorType) == requiredInterceptorType;
        }

        private Type GetAttributeType(Type factoryImpl)
        {
            var factory = TypeHierarchyScanner.GetInterfaces(factoryImpl)
                .Single(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IAttributeInterceptor<>));

            return factory.GetGenericArguments().Single();
        }
    }
}
