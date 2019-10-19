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
    /// Resolve all the dependencies found in all type properties.
    /// </summary>
    [Component]
    public class PropertyFill
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        [Dependency]
        public ValueProvider ValueProvider { get; set; }

        [Dependency]
        public DependencyExceptionFactory DependencyExceptionHandler { get; set; }

        private IDictionary<Type, object> _resolveStrategies = new Dictionary<Type, object>();

        private static MethodInfo InvokeStrategyMethod = typeof(PropertyFill)
            .GetMethod(nameof(InvokeStrategy), BindingFlags.NonPublic | BindingFlags.Instance);

        public PropertyFill()
        {
            AddDefaultResolveStrategies();
        }

        protected virtual void AddDefaultResolveStrategies()
        {
            AddResolveStrategy<DependencyAttribute>((prop, attr, containerReg) =>
            {
                return containerReg.Resolve(prop.PropertyType, attr.Name);
            });

            AddResolveStrategy<OptionalDependencyAttribute>((prop, attr, containerReg) =>
            {
                return containerReg.IsRegistered(prop.PropertyType, attr.Name) ?
                    containerReg.Resolve(prop.PropertyType, attr.Name) : null;
            });

            AddResolveStrategy<ValueAttribute>((prop, attr, containerReg) =>
            {
                return ValueProvider.GetValue(prop.PropertyType, attr.Value);
            });
        }

        public void AddResolveStrategy<TAttribute>(Func<PropertyInfo, TAttribute, IContainerRegistry, object> strategy) where TAttribute : Attribute
        {
            _resolveStrategies[typeof(TAttribute)] = strategy;
        }

        private object InvokeStrategy<TAttribute>(Func<PropertyInfo, TAttribute, IContainerRegistry, object> strategy, PropertyInfo prop, TAttribute attr, IContainerRegistry containerReg)
        {
            return strategy(prop, attr, containerReg);
        }

        public object FillAllProperties(object obj)
        {
            foreach (var prop in SelectAllProperties(obj.GetType()))
            {
                InjectDependency(prop, obj);
            }

            return obj;
        }

        public void InjectDependency(PropertyInfo prop, object obj)
        {
            if (prop.SetMethod == null)
            {
                return;
            }

            try
            {
                foreach (var propAttr in prop.GetCustomAttributes(false))
                {
                    var attrType = propAttr.GetType();

                    if (_resolveStrategies.ContainsKey(attrType))
                    {
                        prop.SetMethod.Invoke(obj, new object[] { InvokeStrategyMethod.MakeGenericMethod(attrType).Invoke(this, new object[] { _resolveStrategies[attrType], prop, prop.GetAttribute(attrType), ContainerRegistry }) });
                        break;
                    }
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException is NoSuchBeanDefinitionException)
            {
                throw DependencyExceptionHandler.CreateException(prop, (dynamic)ex.InnerException);
            }
        }

        public static IEnumerable<PropertyInfo> SelectAllProperties(Type type)
        {
            ISet<PropertyInfo> props = new HashSet<PropertyInfo>();

            while (type != null)
            {
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    props.Add(prop);
                }

                type = type.BaseType;
            }

            return props;
        }
    }
}
