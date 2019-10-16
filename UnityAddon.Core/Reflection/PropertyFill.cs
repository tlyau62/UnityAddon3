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

        private IDictionary<Type, object> _resolveStrategies = new Dictionary<Type, object>();

        public PropertyFill()
        {
            AddDefaultResolveStrategies();
        }

        public void AddDefaultResolveStrategies()
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
            var invokeStrategy = GetType().GetMethod("InvokeStrategy", BindingFlags.NonPublic | BindingFlags.Instance);
            var type = obj.GetType();

            try
            {
                foreach (var prop in SelectAllProperties(type))
                {
                    if (prop.SetMethod == null)
                    {
                        continue;
                    }

                    foreach (var strategy in _resolveStrategies)
                    {
                        if (prop.HasAttribute(strategy.Key))
                        {
                            prop.SetMethod.Invoke(obj, new object[] {
                                invokeStrategy.MakeGenericMethod(strategy.Key).Invoke(this, new object[] { strategy.Value, prop, prop.GetAttribute(strategy.Key), ContainerRegistry })
                            });
                            break;
                        }
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is NoSuchBeanDefinitionException)
                {
                    throw ex.InnerException;
                }
                throw ex;
            }

            return obj;
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
