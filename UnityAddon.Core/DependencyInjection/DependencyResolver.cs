using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.DependencyInjection
{
    //TODO:[Component]
    public class DependencyResolver
    {
        [Dependency]
        public ValueProvider ValueProvider { get; set; }

        private IDictionary<Type, object> _resolveStrategies = new Dictionary<Type, object>();

        private static MethodInfo InvokeStrategyMethod = typeof(DependencyResolver)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

        public DependencyResolver()
        {
            AddDefaultResolveStrategies();
        }

        protected virtual void AddDefaultResolveStrategies()
        {
            AddResolveStrategy<DependencyAttribute>((type, attr, container) =>
            {
                return container.ResolveUA(type, attr.Name);
            });

            AddResolveStrategy<OptionalDependencyAttribute>((type, attr, container) =>
            {
                return container.IsRegistered(type, attr.Name) ?
                    container.ResolveUA(type, attr.Name) : null;
            });

            AddResolveStrategy<ValueAttribute>((type, attr, containerReg) =>
            {
                return ValueProvider.GetValue(type, attr.Value);
            });
        }

        public object Resolve(Type resolveType, IEnumerable<Attribute> attributes, IUnityContainer container)
        {
            try
            {
                foreach (var attribute in attributes)
                {
                    var attrType = attribute.GetType();

                    if (_resolveStrategies.ContainsKey(attrType))
                    {
                        return InvokeStrategyMethod.MakeGenericMethod(attrType).Invoke(this, new object[] { _resolveStrategies[attrType], resolveType, attribute, container });
                    }
                }

                return null;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public void AddResolveStrategy<TAttribute>(Func<Type, TAttribute, IUnityContainer, object> strategy) where TAttribute : Attribute
        {
            _resolveStrategies[typeof(TAttribute)] = strategy;
        }

        private object InvokeStrategy<TAttribute>(Func<Type, TAttribute, IUnityContainer, object> strategy, Type type, TAttribute attr, IUnityContainer container)
        {
            return strategy(type, attr, container);
        }
    }

}
