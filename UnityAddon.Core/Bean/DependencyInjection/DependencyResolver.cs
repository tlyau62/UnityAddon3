using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    public class DependencyResolver
    {
        private readonly IDictionary<Type, object> _resolveStrategies;

        private static readonly MethodInfo InvokeStrategyMethod = typeof(DependencyResolver)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

        public DependencyResolver()
        {
            _resolveStrategies = new Dictionary<Type, object>();

            AddInternalResolveStrategies();
        }

        protected void AddInternalResolveStrategies()
        {
            AddResolveStrategy<DependencyAttribute>((type, attr, sp) =>
            {
                return sp.GetRequiredService(type, attr.Name);
            });

            AddResolveStrategy<OptionalDependencyAttribute>((type, attr, sp) =>
            {
                return sp.GetService(type, attr.Name);
            });

            AddResolveStrategy<ValueAttribute>((type, attr, sp) =>
            {
                return sp.GetService<ValueProvider>().GetValue(type, attr.Value);
            });
        }

        public void AddResolveStrategy<TAttribute>(Func<Type, TAttribute, IServiceProvider, object> strategy) where TAttribute : Attribute
        {
            _resolveStrategies[typeof(TAttribute)] = strategy;
        }

        public object Resolve(Type resolveType, IEnumerable<Attribute> attributes, IServiceProvider sp)
        {
            try
            {
                foreach (var attribute in attributes)
                {
                    var attrType = attribute.GetType();

                    if (_resolveStrategies.ContainsKey(attrType))
                    {
                        return InvokeStrategyMethod.MakeGenericMethod(attrType).Invoke(this, new object[] { _resolveStrategies[attrType], resolveType, attribute, sp });
                    }
                }

                return null;
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

                throw;
            }
        }

        private object InvokeStrategy<TAttribute>(Func<Type, TAttribute, IServiceProvider, object> strategy, Type type, TAttribute attr, IServiceProvider sp)
        {
            return strategy(type, attr, sp);
        }
    }

}
