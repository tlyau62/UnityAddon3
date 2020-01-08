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

namespace UnityAddon.Core.DependencyInjection
{
    /// <summary>
    /// Resolve all the dependencies found in all type properties.
    /// </summary>
    [Component]
    public class PropertyFill
    {
        [Dependency]
        public DependencyExceptionFactory DependencyExceptionHandler { get; set; }

        [Dependency]
        public DependencyResolver DependencyResolver { get; set; }

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
                var dep = DependencyResolver.Resolve(prop.PropertyType, prop.GetCustomAttributes(false).Cast<Attribute>());

                // must add null check, else something will be wrong.
                if (dep != null)
                {
                    prop.SetMethod.Invoke(obj, new[] { dep });
                }
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                throw DependencyExceptionHandler.CreateException(prop, (dynamic)ex);
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
