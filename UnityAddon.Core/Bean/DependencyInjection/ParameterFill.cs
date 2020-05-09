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
using Microsoft.Extensions.DependencyInjection;

namespace UnityAddon.Core.Bean.DependencyInjection
{
    /// <summary>
    /// Resolve all the dependencies found in a method parameters.
    /// </summary>
    [Component]
    public class ParameterFill
    {
        [Dependency]
        public DependencyResolver DependencyResolver { get; set; }

        public object[] FillAllParamaters(MethodBase method, IUnityAddonSP sp)
        {
            return method.GetParameters().Select(param => GetDependency(param, sp)).ToArray();
        }

        public object GetDependency(ParameterInfo param, IUnityAddonSP sp)
        {
            var attrs = param.GetCustomAttributes(false).Cast<Attribute>();

            try
            {
                if (attrs.Count() == 0)
                {
                    return sp.GetRequiredService(param.ParameterType);
                }

                return DependencyResolver.Resolve(param.ParameterType, attrs, sp);
            }
            catch (NoSuchBeanDefinitionException ex)
            {
                throw new NoSuchBeanDefinitionException(
                    $"Parameter {param.Position} of Constructor in {param.Member} required a bean of type '{param.ParameterType}' that could not be found.",
                    ex);
            }
        }

        public bool CanResolve(ParameterInfo param)
        {
            return param.CustomAttributes.Any(p => DependencyResolver.ContainAttribute(p.AttributeType));
        }
    }
}
