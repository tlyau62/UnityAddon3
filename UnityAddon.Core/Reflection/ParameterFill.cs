using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.Reflection
{
    /// <summary>
    /// Resolve all the dependencies found in a method parameters.
    /// </summary>
    [Component]
    public class ParameterFill
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public ValueProvider ValueProvider { get; set; }

        [Dependency]
        public DependencyExceptionFactory DependencyExceptionHandler { get; set; }

        private IDictionary<Type, object> _resolveStrategies = new Dictionary<Type, object>();

        private static MethodInfo InvokeStrategyMethod = typeof(ParameterFill)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

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

            AddResolveStrategy<ValueAttribute>((param, attr, containerReg) =>
            {
                return ValueProvider.GetValue(param.ParameterType, attr.Value);
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

            return method.GetParameters().Select(param => GetDependency(param)).ToArray();
        }

        public object GetDependency(ParameterInfo param)
        {
            try
            {
                foreach (var paramAttr in param.GetCustomAttributes(false))
                {
                    var attrType = paramAttr.GetType();

                    if (_resolveStrategies.ContainsKey(attrType))
                    {
                        return InvokeStrategyMethod.MakeGenericMethod(attrType).Invoke(this, new object[] { _resolveStrategies[attrType], param, param.GetAttribute(attrType), ContainerRegistry });
                    }
                }

                return ContainerRegistry.Resolve(param.ParameterType, null);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is NoSuchBeanDefinitionException)
            {
                throw DependencyExceptionHandler.CreateException(param, (dynamic)ex.InnerException);
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                throw DependencyExceptionHandler.CreateException(param, (dynamic)ex);
            }
        }
    }
}
