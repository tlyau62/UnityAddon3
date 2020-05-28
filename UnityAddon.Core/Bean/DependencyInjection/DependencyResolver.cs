using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Context;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    public class DependencyResolver : IAppCtxPostServiceRegistrationPhase, IAppCtxPreInstantiateSingletonPhase
    {
        private IDictionary<Type, object> _resolveStrategies;

        private static readonly MethodInfo InvokeStrategyMethod = typeof(DependencyResolver)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

        public DependencyResolver()
        {
            AddInternalResolveStrategies();
        }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        public void Process()
        {
            foreach (var option in Sp.GetServices<DependencyResolverOption>())
            {
                foreach (var entry in option.ResolveStrategies)
                {
                    _resolveStrategies[entry.Key] = entry.Value;
                }
            }
        }

        protected void AddInternalResolveStrategies()
        {
            var defaultOption = new DependencyResolverOption();

            defaultOption.AddResolveStrategy<DependencyAttribute>((type, attr, sp) =>
            {
                return sp.GetRequiredService(type, attr.Name);
            });

            defaultOption.AddResolveStrategy<OptionalDependencyAttribute>((type, attr, sp) =>
            {
                return sp.GetService(type, attr.Name);
            });

            _resolveStrategies = defaultOption.ResolveStrategies;
        }

        public object Resolve(Type resolveType, IEnumerable<Attribute> attributes, IUnityAddonSP sp)
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

        public bool ContainAttribute(Type attributeType)
        {
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException("Given value is not attribute type");
            }

            return _resolveStrategies.ContainsKey(attributeType);
        }

        private object InvokeStrategy<TAttribute>(Func<Type, TAttribute, IUnityAddonSP, object> strategy, Type type, TAttribute attr, IUnityAddonSP sp)
        {
            return strategy(type, attr, sp);
        }
    }

}
