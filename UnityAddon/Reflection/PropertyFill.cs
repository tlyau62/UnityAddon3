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
        public static object FillAllProperties(object obj, IContainerRegistry containerRegistry)
        {
            var type = obj.GetType();
            var props = SelectAllProperties(type)
                .Where(m => m.HasAttribute<DependencyResolutionAttribute>(true) && m.SetMethod != null);

            foreach (var prop in props)
            {
                if (prop.HasAttribute<DependencyAttribute>())
                {
                    var depAttr = prop.GetAttribute<DependencyAttribute>();

                    prop.SetMethod.Invoke(obj, new object[] {
                        containerRegistry.Resolve(prop.PropertyType, depAttr.Name)
                    });
                }
                else if (prop.HasAttribute<OptionalDependencyAttribute>())
                {
                    var optDepAttr = prop.GetAttribute<OptionalDependencyAttribute>();

                    prop.SetMethod.Invoke(obj, new object[] {
                        containerRegistry.IsRegistered(prop.PropertyType, optDepAttr.Name) ?
                            containerRegistry.Resolve(prop.PropertyType, optDepAttr.Name) : null});
                }
                else
                {
                    throw new NotImplementedException();
                }
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
