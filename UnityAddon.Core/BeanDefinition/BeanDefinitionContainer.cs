using UnityAddon.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Reflection;
using System.Collections.Concurrent;
using Unity;
using UnityAddon.Core.DependencyInjection;
using UnityAddon.Core.Thread;
using Castle.DynamicProxy;
using UnityAddon.Core.BeanBuildStrategies;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionContainer
    {
        bool HasBeanDefinition(Type type, string name = null);

        IBeanDefinition GetBeanDefinition(Type type, string name = null);

        IEnumerable<IBeanDefinition> GetAllBeanDefinitions(Type type);

        IEnumerable<IBeanDefinition> GetAllGenericBeanDefinitionsByTypeDefinition(Type type);

        IBeanDefinitionContainer RegisterBeanDefinition(IBeanDefinition beanDefinition);

        void RegisterBeanDefinitions(IEnumerable<IBeanDefinition> beanDefinitions);

        IEnumerable<IBeanDefinition> FindBeanDefinitionsByAttribute<TAttribute>() where TAttribute : Attribute;

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

        public BeanDefinitionContainer()
        {
            RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IBeanDefinitionContainer)));
            RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IUnityContainer)));
            RegisterBeanDefinition(new SimpleBeanDefinition(typeof(DependencyResolver)));
            RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IThreadLocalFactory<Stack<IInvocation>>)));
            RegisterBeanDefinition(new SimpleBeanDefinition(typeof(IThreadLocalFactory<Stack<ResolveStackEntry>>)));
        }

        public IBeanDefinitionContainer RegisterBeanDefinition(IBeanDefinition beanDefinition)
        {
            if (beanDefinition.RequireAssignableTypes)
            {
                // bad time and space
                foreach (var type in TypeResolver.GetAssignableTypes(beanDefinition.BeanType))
                {
                    AddBeanDefinition(type, beanDefinition);
                }
            }
            else
            {
                AddBeanDefinition(beanDefinition.BeanType, beanDefinition);
            }

            return this;
        }

        private void AddBeanDefinition(Type type, IBeanDefinition beanDefinition)
        {
            if (!_container.ContainsKey(type))
            {
                _container[type] = new BeanDefinitionHolder();
            }

            _container[type].Add(beanDefinition);
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
        public IBeanDefinition GetBeanDefinition(Type type, string name)
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
        public IEnumerable<IBeanDefinition> FindBeanDefinitionsByAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _container
                .Where(ent => ent.Key.HasAttribute<TAttribute>()) // find all implementation types
                .Select(ent => ent.Value.Get()); // implementation type must have only 1 bean definition
        }

        public IEnumerable<IBeanDefinition> GetAllGenericBeanDefinitionsByTypeDefinition(Type type)
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

            if (beanDef is TypeBeanDefinition)
            {
                foreach (var atype in TypeResolver.GetAssignableTypes(beanDef.BeanType))
                {
                    _container[atype].Remove(beanDef);
                }
            }
            else
            {
                _container[type].Remove(beanDef);
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
