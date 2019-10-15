using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Attributes;
using UnityAddon.Bean;
using UnityAddon.Reflection;

namespace UnityAddon
{
    /// <summary>
    /// Parse bean methods in a configuration class into bean definitions
    /// </summary>
    [Component]
    public class ConfigurationParser
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanDefinitionRegistry BeanDefinitionRegistry { get; set; }

        private ISet<Type> _scannedConfigs = new HashSet<Type>();

        public void ParseScannedConfigurations()
        {
            var configs = BeanDefinitionContainer.FindBeanDefinitionsByAttribute<ConfigurationAttribute>().ToList();

            foreach (var config in configs)
            {
                ParseConfiguration(config.GetBeanType());
            }
        }

        public void ParseConfiguration(Type type)
        {
            if (_scannedConfigs.Contains(type))
            {
                return;
            }

            foreach (var beanMethod in MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(type))
            {
                BeanDefinitionRegistry.Register(new MethodBeanDefinition(beanMethod));
            }

            _scannedConfigs.Add(type);
        }
    }
}
