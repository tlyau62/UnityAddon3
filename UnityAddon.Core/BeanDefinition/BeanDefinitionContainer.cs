using UnityAddon.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Reflection;
using System.Collections.Concurrent;
using Unity;
using Castle.DynamicProxy;
using UnityAddon.Core.BeanBuildStrategies;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionContainer
    {
        bool HasBeanDefinition(Type type, string name = null);

        IBeanDefinition GetBeanDefinition(Type type, string name = null);

        IEnumerable<IBeanDefinition> GetAllBeanDefinitions(Type type);

        IBeanDefinitionContainer RegisterBeanDefinition(IBeanDefinition beanDefinition);

        void RegisterBeanDefinitions(IEnumerable<IBeanDefinition> beanDefinitions);

        IBeanDefinition RemoveBeanDefinition(Type type, string name = null);
    }

    /// <summary>
    /// Hold all the bean definition scanned after component scanning.
    /// Thread-safe.
    /// This class is not component scanned and thus required manually registered.
    /// </summary>
    public class BeanDefinitionContainer : IBeanDefinitionContainer
    {
        private ConcurrentDictionary<Type, BeanDefinitionHolder> _container = new ConcurrentDictionary<Type, BeanDefinitionHolder>();

        public IBeanDefinitionContainer RegisterBeanDefinition(IBeanDefinition beanDefinition)
        {
            foreach (var type in beanDefinition.AutoWiredTypes)
            {
                if (!_container.ContainsKey(type))
                {
                    _container[type] = new BeanDefinitionHolder();
                }

                _container[type].Add(beanDefinition);
            }

            return this;
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
        /// </summary>
        public IBeanDefinition GetBeanDefinition(Type type, string name)
        {
            if (!HasBeanDefinition(type, name))
            {
                throw new InvalidOperationException($"No such bean definition of type {type}.");
            }

            return _container[type].Get(name);
        }

        public IEnumerable<IBeanDefinition> GetAllBeanDefinitions(Type type)
        {
            if (!_container.ContainsKey(type))
            {
                return new List<IBeanDefinition>();
            }

            return _container[type].GetAll();
        }

        public IBeanDefinition RemoveBeanDefinition(Type type, string name = null)
        {
            var beanDef = GetBeanDefinition(type, name);

            foreach (var atype in beanDef.AutoWiredTypes)
            {
                _container[atype].Remove(beanDef);
            }

            return beanDef;
        }

        public void RegisterBeanDefinitions(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            foreach (var def in beanDefinitions)
            {
                RegisterBeanDefinition(def);
            }
        }
    }
}
