using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.DependencyInjection
{
    /// <summary>
    /// Resolve all the dependencies found in a method parameters.
    /// </summary>
    //TODO:[Component]
    public class ParameterFill
    {
        [Dependency]
        public DependencyResolver DependencyResolver { get; set; }

        public object[] FillAllParamaters(MethodBase method, IUnityContainer container)
        {
            return method.GetParameters().Select(param => GetDependency(param, container)).ToArray();
        }

        public object GetDependency(ParameterInfo param, IUnityContainer container)
        {
            try
            {
                var dep = DependencyResolver.Resolve(param.ParameterType, param.GetCustomAttributes(false).Cast<Attribute>(), container);

                return dep ?? (param.HasAttribute<OptionalDependencyAttribute>() ? null : container.Resolve(param.ParameterType, null));
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                throw new BeanCreationException(param, (dynamic)ex);
            }
        }
    }
}
