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
        public DependencyExceptionFactory DependencyExceptionHandler { get; set; }

        [Dependency]
        public DependencyResolver DependencyResolver { get; set; }

        public object[] FillAllParamaters(MethodBase method)
        {
            return method.GetParameters().Select(param => GetDependency(param)).ToArray();
        }

        public object GetDependency(ParameterInfo param)
        {
            try
            {
                var dep = DependencyResolver.Resolve(param.ParameterType, param.GetCustomAttributes(false).Cast<Attribute>());

                return dep ?? (param.HasAttribute<OptionalDependencyAttribute>() ? null : ContainerRegistry.Resolve(param.ParameterType, null));
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                throw DependencyExceptionHandler.CreateException(param, (dynamic)ex);
            }
        }
    }
}
