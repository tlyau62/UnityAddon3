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

        public object FillAllProperties(object obj, IServiceProvider sp)
        {
            foreach (var prop in SelectAllProperties(obj.GetType()))
            {
                InjectDependency(prop, obj, sp);
            }

            return obj;
        }

        public void InjectDependency(PropertyInfo prop, object obj, IServiceProvider sp)
        {
            if (prop.SetMethod == null)
            {
                return;
            }

            var dep = DependencyResolver.Resolve(prop.PropertyType, prop.GetCustomAttributes(false).Cast<Attribute>(), sp);

            // must add null check, else something will be wrong.
            if (dep != null)
            {
                prop.SetMethod.Invoke(obj, new[] { dep });
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
