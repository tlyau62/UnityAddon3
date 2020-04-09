using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.DependencyInjection
{
    public class DependencyResolverBuilder
    {
        private readonly IDictionary<Type, object> _resolveStrategies;

        public DependencyResolverBuilder()
        {
            _resolveStrategies = new Dictionary<Type, object>();

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

        public DependencyResolver Build()
        {
            return new DependencyResolver(_resolveStrategies);
        }
    }
}
