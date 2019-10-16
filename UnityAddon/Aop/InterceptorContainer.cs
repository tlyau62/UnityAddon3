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

        public InterceptorContainer()
        {
            _interceptorFactoryMethod = GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(CreateInterceptor) && m.IsGenericMethod).Single();
        }

        public void Build()
        {
            if (!BeanDefinitionContainer.HasBeanDefinition(typeof(IInterceptorFactory<>)))
            {
                return;
            }

            foreach (var beanDef in BeanDefinitionContainer.GetAllBeanDefinitions(typeof(IInterceptorFactory<>)))
            {
                var interceptorAttribute = GetAttributeType(beanDef.GetBeanType());
                IInterceptor interceptor = CreateInterceptor(beanDef, interceptorAttribute);

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

        private IInterceptor CreateInterceptor<TAttribute>(IInterceptorFactory<TAttribute> factory) where TAttribute : Attribute
        {
            return factory.CreateInterceptor();
        }

        private IInterceptor CreateInterceptor(AbstractBeanDefinition beanDefinition, Type interceptorAttribute)
        {
            return (IInterceptor)_interceptorFactoryMethod
                .MakeGenericMethod(interceptorAttribute)
                .Invoke(this, new[] { ContainerRegistry.Resolve(beanDefinition.GetBeanType(), beanDefinition.GetBeanName()) });
        }

        private Type GetAttributeType(Type factoryImpl)
        {
            var factory = TypeHierarchyScanner.GetInterfaces(factoryImpl)
                .Single(itf => itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IInterceptorFactory<>));

            return factory.GetGenericArguments().Single();
        }
    }
}
