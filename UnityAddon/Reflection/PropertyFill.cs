using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Reflection
{
    public class PropertyFill
    {
        public static object FillAllProperties(object obj, IUnityContainer container)
        {
            var type = obj.GetType();
            var props = SelectAllProperties(type)
                .Where(m => m.HasAttribute<DependencyAttribute>() && m.SetMethod != null);

            foreach (var prop in props)
            {
                prop.SetMethod.Invoke(obj, new object[] {
                    container.Resolve(prop.PropertyType, prop.GetAttribute<DependencyAttribute>().Name)
                });
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
