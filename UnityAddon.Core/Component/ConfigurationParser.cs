using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Component
{
    /// <summary>
    /// Parse bean methods in a configuration class into bean definitions
    /// </summary>
    public class ConfigurationParser
    {
        public IEnumerable<IBeanDefinition> Parse(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            return beanDefinitions
                .Where(def => def is TypeBeanDefinition typeDef && typeDef.IsConfiguration)
                .Cast<TypeBeanDefinition>()
                .SelectMany(def => MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(def.Type))
                .Select(beanMethod => new MethodBeanDefinition(beanMethod));
        }
    }
}
