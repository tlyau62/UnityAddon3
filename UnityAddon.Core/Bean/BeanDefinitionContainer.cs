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
        IEnumerable<AbstractBeanDefinition> GetAllGenericBeanDefinitionsByTypeDefinition(Type type);
        void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition);
        IEnumerable<AbstractBeanDefinition> FindBeanDefinitionsByAttribute<TAttribute>() where TAttribute : Attribute;
        void Clear();
        AbstractBeanDefinition RemoveBeanDefinition(Type type, string name = null);
    }

    /// <summary>
    /// Hold all the bean definition scanned after component scanning.
    /// Thread-safe.
    /// This class is not component scanned and thus required manually registered.
    /// </summary>
    public class BeanDefinitionContainer : IBeanDefinitionContainer
    {
        private IDictionary<Type, BeanDefinitionHolder> _container = new ConcurrentDictionary<Type, BeanDefinitionHolder>();

        // bad time and space
        public void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition)
        {
            foreach (var type in TypeResolver.GetAssignableTypes(beanDefinition.BeanType))
            {
                if (!_container.ContainsKey(type))
                {
                    _container[type] = new BeanDefinitionHolder();
                }

                _container[type].Add(beanDefinition);
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

        public IEnumerable<AbstractBeanDefinition> GetAllGenericBeanDefinitionsByTypeDefinition(Type type)
        {
            if (!type.IsGenericType || !type.IsTypeDefinition)
            {
                throw new InvalidOperationException("Not generic type definition.");
            }

            return _container
                .Where(ent => ent.Key == type || (ent.Key.IsGenericType && ent.Key.GetGenericTypeDefinition() == type))
                .SelectMany(ent => ent.Value.GetAll())
                .Distinct();
        }

        public IEnumerable<AbstractBeanDefinition> GetAllBeanDefinitions(Type type)
        {
            if (!_container.ContainsKey(type))
            {
                return new List<AbstractBeanDefinition>();
            }

            return _container[type].GetAll();
        }

        public void Clear()
        {
            _container = new ConcurrentDictionary<Type, BeanDefinitionHolder>();
        }

        public AbstractBeanDefinition RemoveBeanDefinition(Type type, string name = null)
        {
            var beanDef = GetBeanDefinition(type, name);

            foreach (var atype in TypeResolver.GetAssignableTypes(beanDef.BeanType))
            {
                _container[atype].Remove(beanDef);
            }

            return beanDef;
        }
    }
}
