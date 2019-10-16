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

        public IDictionary<Type, IList<IInterceptor>> InterceptorMap { get; set; } = new Dictionary<Type, IList<IInterceptor>>();

        private MethodInfo _interceptorFactoryMethod;

        public void Build()
        {
            if (!BeanDefinitionContainer.HasBeanDefinition(typeof(IAttributeInterceptor<>)))
            {
                return;
            }

            foreach (var beanDef in BeanDefinitionContainer.GetAllBeanDefinitions(typeof(IAttributeInterceptor<>)))
            {
                var interceptorAttribute = GetAttributeType(beanDef.GetBeanType());
                IInterceptor interceptor = (IInterceptor)ContainerRegistry.Resolve(beanDef.GetBeanType(), beanDef.GetBeanName());

                if (InterceptorMap.ContainsKey(interceptorAttribute))
                {
                    InterceptorMap[interceptorAttribute].Add(interceptor);
                }
                else
                {
                    InterceptorMap[interceptorAttribute] = new List<IInterceptor>() { interceptor };
                }
            }
        }

        public IDictionary<Type, IList<IInterceptor>> GetClassInterceptorsDictionary()
        {
            return InterceptorMap.Where(entry => IsClassInterceptor(entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        public IDictionary<Type, IList<IInterceptor>> GetMethodInterceptorsDictionary()
        {
            return InterceptorMap.Where(entry => IsMethodInterceptor(entry.Key.GetAttribute<AttributeUsageAttribute>().ValidOn))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }

        private bool IsMethodInterceptor(AttributeTargets attributeTargets)
        {
            return (attributeTargets & AttributeTargets.Method) == AttributeTargets.Method;
        }

        private bool IsClassInterceptor(AttributeTargets attributeTargets)
        {
            return (attributeTargets & AttributeTargets.Method) == AttributeTargets.Class;
        }

        private Type GetAttributeType(Type factoryImpl)
        {
            var factory = TypeHierarchyScanner.GetInterfaces(factoryImpl)
                .Single(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IAttributeInterceptor<>));

            return factory.GetGenericArguments().Single();
        }
    }
}
