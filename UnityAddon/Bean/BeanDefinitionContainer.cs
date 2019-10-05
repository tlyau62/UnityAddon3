using UnityAddon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Bean
{
    public interface IBeanDefinitionContainer
    {
        bool HasBeanDefinition(Type type, string name = null);
        AbstractBeanDefinition GetBeanDefinition(Type type, string name = null);
        void RegisterBeanDefinition(AbstractBeanDefinition beanDefinition);
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

        public AbstractBeanDefinition GetBeanDefinition(Type type, string name)
        {
            if (!_container.ContainsKey(type))
            {
                throw new InvalidOperationException();
            }

            return _container[type].Get(name);
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

            return types;
        }
    }
}
