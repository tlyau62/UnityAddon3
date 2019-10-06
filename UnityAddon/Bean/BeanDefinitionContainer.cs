using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Reflection;

namespace UnityAddon.Bean
{
    public interface IBeanDefinitionContainer
    {
        bool HasBeanDefinition(Type type, string name = null);
        AbstractBeanDefinition GetBeanDefinition(Type type, string name = null);
        void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition);
        IEnumerable<AbstractBeanDefinition> FindBeanDefinitionByAttribute<TAttribute>() where TAttribute : Attribute;
    }

    [Component]
    public class BeanDefinitionContainer : IBeanDefinitionContainer
    {
        private IDictionary<Type, BeanDefinitionHolder> _container = new Dictionary<Type, BeanDefinitionHolder>();

        // bad memory and performance
        public void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition)
        {
            foreach (var assignableType in GetAllAssignableTypes(beanDefinition.GetBeanType()))
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

            return _container[type].Exist(name);
        }

        /// <summary>
        /// Name starting with # is special name used in resolution.
        /// It refers to the same bean definition that can be resolved with "name = null".
        /// The only difference between starting with a '#' and without is the bean factory method.
        /// </summary>
        public AbstractBeanDefinition GetBeanDefinition(Type type, string name)
        {
            if (!_container.ContainsKey(type))
            {
                throw new InvalidOperationException();
            }

            if (name != null && name.StartsWith("#"))
            {
                name = null;
            }

            return _container[type].Get(name);
        }

        /// <summary>
        /// Find all implementation types by attribute
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AbstractBeanDefinition> FindBeanDefinitionByAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _container
                .Where(ent => ent.Key.HasAttribute<TAttribute>()) // find all implementation types
                .Select(ent => ent.Value.Get()); // implementation type must have only 1 bean definition
        }

        private IEnumerable<Type> GetAllAssignableTypes(Type type)
        {
            var types = new List<Type> { type };

            types.AddRange(type.GetInterfaces());

            while (type.BaseType != null && type.BaseType != typeof(object))
            {
                types.Add(type.BaseType);
                type = type.BaseType;
            }

            return types
                .Select(t => Type.GetType($"{t.Namespace}.{t.Name}, {t.Assembly.FullName}")) // reload type
                .Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t); // ensure all generic type are generic def
        }
    }
}
