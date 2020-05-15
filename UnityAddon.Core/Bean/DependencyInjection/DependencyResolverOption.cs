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

        public void AddResolveStrategy<TAttribute>(Func<Type, TAttribute, IUnityAddonSP, object> strategy) where TAttribute : Attribute
        {
            ResolveStrategies[typeof(TAttribute)] = strategy;
        }
    }
}
