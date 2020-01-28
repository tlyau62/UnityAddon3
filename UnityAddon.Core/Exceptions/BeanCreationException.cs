using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Wrap NoSuchBeanDefinitionException with additional property and paramter info.
    /// </summary>
    public class BeanCreationException : Exception
    {
        public BeanCreationException(PropertyInfo prop, NoUniqueBeanDefinitionException ex) : base(
            $"Property {prop.Name} in {prop.DeclaringType.FullName} required a single bean, " +
            $"but {ex.BeanDefinitionHolder.GetAll().Count()} were found:\r\n{ex.BeanDefinitionHolder}",
            ex)
        {
        }

        public BeanCreationException(ParameterInfo param, NoUniqueBeanDefinitionException ex) : base(
            $"Parameter {param.Position} of {param.Member.MemberType} in {param.Member.DeclaringType} required a single bean, " +
            $"but {ex.BeanDefinitionHolder.GetAll().Count()} were found:\r\n{ex.BeanDefinitionHolder}",
            ex)
        {
        }

        public BeanCreationException(PropertyInfo prop, NoSuchBeanDefinitionException ex) : base(
            $"Property {prop.Name} in {prop.DeclaringType.FullName} required a bean of type '{prop.PropertyType.FullName}' that could not be found.",
            ex)
        {
        }

        public BeanCreationException(ParameterInfo param, NoSuchBeanDefinitionException ex) : base(
            $"Parameter {param.Position} of {param.Member.MemberType} in {param.Member.DeclaringType} required a bean of type '{param.ParameterType.FullName}' that could not be found.",
            ex)
        {
        }
    }
}
