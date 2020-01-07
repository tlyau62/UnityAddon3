﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core.DependencyInjection
{
    [Component]
    public class DependencyExceptionFactory
    {
        public NoUniqueBeanDefinitionException CreateException(PropertyInfo prop, NoUniqueBeanDefinitionException ex)
        {
            var beanDefHolder = ex.BeanDefinitionHolder;
            var beanDefsFound = beanDefHolder.GetAll();

            return new NoUniqueBeanDefinitionException(
                $"Property {prop.Name} in {prop.DeclaringType.FullName} required a single bean, " +
                $"but {beanDefsFound.Count()} were found:\r\n{beanDefHolder}");
        }

        public NoUniqueBeanDefinitionException CreateException(ParameterInfo param, NoUniqueBeanDefinitionException ex)
        {
            var beanDefHolder = ex.BeanDefinitionHolder;
            var beanDefsFound = beanDefHolder.GetAll();

            return new NoUniqueBeanDefinitionException(
                $"Parameter {param.Position} of {param.Member.MemberType} in {param.Member.DeclaringType} required a single bean, " +
                $"but {beanDefsFound.Count()} were found:\r\n{beanDefHolder}");
        }

        public NoSuchBeanDefinitionException CreateException(PropertyInfo prop, NoSuchBeanDefinitionException ex)
        {
            return new NoSuchBeanDefinitionException(
                $"Property {prop.Name} in {prop.DeclaringType.FullName} required a bean of type '{prop.PropertyType.FullName}' that could not be found.");
        }

        public NoSuchBeanDefinitionException CreateException(ParameterInfo param, NoSuchBeanDefinitionException ex)
        {
            return new NoSuchBeanDefinitionException(
                $"Parameter {param.Position} of {param.Member.MemberType} in {param.Member.DeclaringType} required a bean of type '{param.ParameterType.FullName}' that could not be found.");
        }
    }
}