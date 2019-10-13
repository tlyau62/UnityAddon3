using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Value;

namespace UnityAddon.Reflection
{
    [Component]
    public class ParameterFill
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public ValueProvider ValueProvider { get; set; }

        private IDictionary<Type, object> _resolveStrategies = new Dictionary<Type, object>();

        public ParameterFill()
        {
            AddDefaultResolveStrategies();
        }

        public void AddDefaultResolveStrategies()
        {
            AddResolveStrategy<DependencyAttribute>((param, attr, containerReg) =>
            {
                return containerReg.Resolve(param.ParameterType, attr.Name);
            });

            AddResolveStrategy<OptionalDependencyAttribute>((param, attr, containerReg) =>
            {
                return containerReg.IsRegistered(param.ParameterType, attr.Name) ?
                    containerReg.Resolve(param.ParameterType, attr.Name) : null;
            });
        }

        public void AddResolveStrategy<TAttribute>(Func<ParameterInfo, TAttribute, IContainerRegistry, object> strategy) where TAttribute : Attribute
        {
            _resolveStrategies[typeof(TAttribute)] = strategy;
        }

        private object InvokeStrategy<TAttribute>(Func<ParameterInfo, TAttribute, IContainerRegistry, object> strategy, ParameterInfo param, TAttribute attr, IContainerRegistry containerReg)
        {
            return strategy(param, attr, containerReg);
        }

        public object[] FillAllParamaters(MethodBase method)
        {
            var invokeStrategy = GetType().GetMethod("InvokeStrategy", BindingFlags.NonPublic | BindingFlags.Instance);

            return method.GetParameters().Select(param =>
            {
                foreach (var strategy in _resolveStrategies)
                {
                    if (param.HasAttribute(strategy.Key))
                    {
                        // create a func type useing GenericMethod
                        return invokeStrategy.MakeGenericMethod(strategy.Key).Invoke(this, new object[] { strategy.Value, param, param.GetAttribute(strategy.Key), ContainerRegistry });
                    }
                }

                return ContainerRegistry.Resolve(param.ParameterType, null);
            })
            .ToArray();
        }
    }
}
