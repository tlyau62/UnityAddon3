using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    public class DependencyResolverOption
    {
        public IDictionary<Type, object> ResolveStrategies { get; } = new Dictionary<Type, object>();

        public DependencyResolverOption(bool useDefault = true)
        {
            if (useDefault)
            {
                AddInternalResolveStrategies();
            }
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

            //AddResolveStrategy<ValueAttribute>((type, attr, sp) =>
            //{
            //    return sp.GetRequiredService<ValueProvider>().GetValue(type, attr.Value);
            //});
        }

        public void AddResolveStrategy<TAttribute>(Func<Type, TAttribute, IServiceProvider, object> strategy) where TAttribute : Attribute
        {
            ResolveStrategies[typeof(TAttribute)] = strategy;
        }
    }
}
