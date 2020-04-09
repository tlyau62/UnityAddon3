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

        public DependencyResolver(IDictionary<Type, object> resolveStrategies)
        {
            _resolveStrategies = resolveStrategies;
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

        private object InvokeStrategy<TAttribute>(Func<Type, TAttribute, IUnityContainer, object> strategy, Type type, TAttribute attr, IUnityContainer container)
        {
            return strategy(type, attr, container);
        }
    }

}
