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
        IReadOnlyDictionary<Type, BeanDefinitionHolder> Registrations { get; }

        bool HasBeanDefinition(Type type, string name = null);

        IBeanDefinition GetBeanDefinition(Type type, string name = null);

        IEnumerable<IBeanDefinition> GetAllBeanDefinitions(Type type, string name = null);

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
        private readonly ConcurrentDictionary<Type, BeanDefinitionHolder> _registrations = new ConcurrentDictionary<Type, BeanDefinitionHolder>();

        public IReadOnlyDictionary<Type, BeanDefinitionHolder> Registrations => _registrations;

        public IBeanDefinitionContainer RegisterBeanDefinition(IBeanDefinition beanDefinition)
        {
            foreach (var type in beanDefinition.AutoWiredTypes)
            {
                if (!_registrations.ContainsKey(type))
                {
                    _registrations[type] = new BeanDefinitionHolder();
                }

                _registrations[type].Add(beanDefinition);
            }

            return this;
        }

        public bool HasBeanDefinition(Type type, string name)
        {
            if (!_registrations.ContainsKey(type))
            {
                return false;
            }

            return _registrations[type].Exist(name);
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

            return _registrations[type].Get(name);
        }

        public IEnumerable<IBeanDefinition> GetAllBeanDefinitions(Type type, string name)
        {
            if (!_registrations.ContainsKey(type))
            {
                return new List<IBeanDefinition>();
            }

            return _registrations[type].GetAll(name);
        }

        public IBeanDefinition RemoveBeanDefinition(Type type, string name = null)
        {
            var beanDef = GetBeanDefinition(type, name);

            foreach (var atype in beanDef.AutoWiredTypes)
            {
                _registrations[atype].Remove(beanDef);
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
