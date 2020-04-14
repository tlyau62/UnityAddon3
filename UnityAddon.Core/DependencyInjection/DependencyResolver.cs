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
    public class DependencyResolver
    {
        private readonly IDictionary<Type, object> _resolveStrategies;

        private static readonly MethodInfo InvokeStrategyMethod = typeof(DependencyResolver)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

        internal DependencyResolver()
        {
            _resolveStrategies = new Dictionary<Type, object>();

            AddInternalResolveStrategies();
        }

        protected void AddInternalResolveStrategies()
        {
            AddResolveStrategy<DependencyAttribute>((type, attr, container) =>
            {
                return container.ResolveUA(type, attr.Name);
            });

            AddResolveStrategy<OptionalDependencyAttribute>((type, attr, container) =>
            {
                return container.ResolveOptionalUA(type, attr.Name);
            });

            AddResolveStrategy<ValueAttribute>((type, attr, container) =>
            {
                return container.Resolve<ValueProvider>().GetValue(type, attr.Value);
            });
        }

        public void AddResolveStrategy<TAttribute>(Func<Type, TAttribute, IUnityContainer, object> strategy) where TAttribute : Attribute
        {
            _resolveStrategies[typeof(TAttribute)] = strategy;
        }

        internal object Resolve(Type resolveType, IEnumerable<Attribute> attributes, IUnityContainer container)
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

        private object InvokeStrategy<TAttribute>(Func<Type, TAttribute, IUnityContainer, object> strategy, Type type, TAttribute attr, IUnityContainer container)
        {
            return strategy(type, attr, container);
        }
    }

}
