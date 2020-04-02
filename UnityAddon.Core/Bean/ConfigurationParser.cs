using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core
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
        public ComponentScanner ComponentScanner { get; set; }

        private ISet<Type> _scannedConfigs = new HashSet<Type>();

        public void ParseScannedConfigurations(IUnityContainer container)
        {
            var configs = BeanDefinitionContainer.FindBeanDefinitionsByAttribute<ConfigurationAttribute>().ToList();

            foreach (var config in configs)
            {
                ParseConfiguration(config.BeanType, container);
            }
        }

        public void ParseConfiguration(Type type, IUnityContainer container)
        {
            if (_scannedConfigs.Contains(type))
            {
                return;
            }

            foreach (var beanMethod in MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(type))
            {
                ComponentScanner.AddComponent(new MethodBeanDefinition(beanMethod), container);
            }

            _scannedConfigs.Add(type);
        }
    }
}
