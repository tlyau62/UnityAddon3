using Castle.DynamicProxy;
using UnityAddon.Attributes;
using UnityAddon.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Bean
{
    [Component]
    public class BeanFactory
    {
        [Dependency]
        public ProxyGenerator ProxyGenerator { get; set; }

        public Func<IUnityContainer, Type, string, object> CreateFactory(Type type)
        {
            if (type.HasAttribute<ComponentAttribute>())
            {
                return new Func<IUnityContainer, Type, string, object>((c, t, n) =>
                {
                    var defaultCtor = DefaultConstructor.Select(type);
                    var obj = defaultCtor.Invoke(ParameterFill.FillAllParamaters(defaultCtor, c));

                    return PropertyFill.FillAllProperties(obj, c);
                });
            }
            else if (type.HasAttribute<ConfigurationAttribute>())
            {
                return new Func<IUnityContainer, Type, string, object>((c, t, n) =>
                {
                    var defaultCtor = DefaultConstructor.Select(type);
                    var ps = ParameterFill.FillAllParamaters(defaultCtor, c);
                    var obj = ProxyGenerator.CreateClassProxy(type, ps);

                    return PropertyFill.FillAllProperties(obj, c);
                });
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
