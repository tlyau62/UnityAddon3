using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.MemberBean;

namespace UnityAddon.Core.Bean
{
    public class ImportRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefContainer { get; set; }

        [Dependency]
        public IServiceRegistry ServiceRegistry { get; set; }

        private readonly HashSet<IBeanDefinition> _parsedConfigs = new HashSet<IBeanDefinition>();

        public void Refresh()
        {
            var importConfigs = BeanDefContainer.Registrations.Values
                .SelectMany(holder => holder.GetAll().Where(def => def is MemberConfigurationBeanDefinition))
                .Where(def =>
                {
                    if (_parsedConfigs.Contains(def))
                    {
                        return false;
                    }

                    _parsedConfigs.Add(def);

                    return true;
                })
                .SelectMany(configBeanDef => ParseImport(configBeanDef.Type))
                .ToArray();

            if (importConfigs.Length == 0)
            {
                return;
            }

            foreach (var config in importConfigs)
            {
                ServiceRegistry.ConfigureBeans(beans => beans.AddConfiguration(config));
            }

            Refresh();
        }

        public IEnumerable<Type> ParseImport(Type configType)
        {
            return configType.GetCustomAttributes(false)
                .Where(attr => attr is ImportAttribute)
                .Cast<ImportAttribute>()
                .SelectMany(attr => attr.Classes);
        }
    }
}
