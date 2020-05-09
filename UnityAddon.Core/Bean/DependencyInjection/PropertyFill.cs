using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    /// <summary>
    /// Resolve all the dependencies found in all type properties.
    /// </summary>
    [Component]
    public class PropertyFill
    {
        [Dependency]
        public DependencyResolver DependencyResolver { get; set; }

        public object FillAllProperties(Type type, object obj, IUnityAddonSP sp)
        {
            foreach (var prop in SelectAllProperties(type))
            {
                InjectDependency(prop, obj, sp);
            }

            return obj;
        }

        public void InjectDependency(PropertyInfo prop, object obj, IUnityAddonSP sp)
        {
            if (prop.SetMethod == null)
            {
                return;
            }

            try
            {
                var dep = DependencyResolver.Resolve(prop.PropertyType, prop.GetCustomAttributes(false).Cast<Attribute>(), sp);

                // must add null check, else something will be wrong.
                if (dep != null)
                {
                    prop.SetMethod.Invoke(obj, new[] { dep });
                }
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                if (ex.InnerException == null)
                {
                    throw new NoSuchBeanDefinitionException($"Property {prop.Name} in {prop.DeclaringType} required a bean of type '{prop.PropertyType}' that could not be found.", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private IEnumerable<PropertyInfo> SelectAllProperties(Type type)
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
