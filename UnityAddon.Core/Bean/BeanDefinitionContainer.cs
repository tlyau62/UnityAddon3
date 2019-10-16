using UnityAddon.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Reflection;
using System.Collections.Concurrent;

namespace UnityAddon.Core.Bean
{
    public interface IBeanDefinitionContainer
    {
        bool HasBeanDefinition(Type type, string name = null);
        AbstractBeanDefinition GetBeanDefinition(Type type, string name = null);
        IEnumerable<AbstractBeanDefinition> GetAllBeanDefinitions(Type type);
        void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition);
        IEnumerable<AbstractBeanDefinition> FindBeanDefinitionsByAttribute<TAttribute>() where TAttribute : Attribute;
        void Clear();
    }

    /// <summary>
    /// Hold all the bean definition scanned after component scanning.
    /// Thread-safe.
    /// This class is not component scanned and thus required manually registered.
    /// </summary>
    public class BeanDefinitionContainer : IBeanDefinitionContainer
    {
        private IDictionary<Type, BeanDefinitionHolder> _container = new ConcurrentDictionary<Type, BeanDefinitionHolder>();

        // bad memory and performance
        public void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition)
        {
            foreach (var assignableType in GetAssignableTypeDefinitions(beanDefinition.GetBeanType()))
            {
                if (!_container.ContainsKey(assignableType))
                {
                    _container[assignableType] = new BeanDefinitionHolder();
                }

                _container[assignableType].Add(beanDefinition);
            }
        }

        public bool HasBeanDefinition(Type type, string name)
        {
            if (!_container.ContainsKey(type))
            {
                return false;
            }

            if (name != null && name.StartsWith("#"))
            {
                name = name.Substring(1);
            }

            return _container[type].Exist(name);
        }

        /// <summary>
        /// Name starting with # is special name used in resolution.
        /// </summary>
        public AbstractBeanDefinition GetBeanDefinition(Type type, string name)
        {
            if (!_container.ContainsKey(type))
            {
                throw new InvalidOperationException($"No such bean definition of type {type}.");
            }

            if (name != null && name.StartsWith("#"))
            {
                name = name.Substring(1);
            }

            return _container[type].Get(name);
        }

        /// <summary>
        /// Find all implementation types by attribute
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AbstractBeanDefinition> FindBeanDefinitionsByAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _container
                .Where(ent => ent.Key.HasAttribute<TAttribute>()) // find all implementation types
                .Select(ent => ent.Value.Get()); // implementation type must have only 1 bean definition
        }

        public IEnumerable<AbstractBeanDefinition> GetAllBeanDefinitions(Type type)
        {
            if (!_container.ContainsKey(type))
            {
                throw new InvalidOperationException($"No such bean definition of type {type}.");
            }

            return _container[type].GetAll();
        }

        private IEnumerable<Type> GetAssignableTypeDefinitions(Type type)
        {
            return TypeHierarchyScanner.GetAssignableTypes(type)
                .Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t); // ensure all generic type are generic def
        }

        public void Clear()
        {
            _container = new ConcurrentDictionary<Type, BeanDefinitionHolder>();
        }
    }
}
