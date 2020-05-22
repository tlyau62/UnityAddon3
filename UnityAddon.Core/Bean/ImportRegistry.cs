using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

        private readonly HashSet<Type> _parsedComponents = new HashSet<Type>();

        public void Refresh()
        {
            var importComps = BeanDefContainer.Registrations.Values
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

            if (importComps.Length == 0)
            {
                return;
            }

            ServiceRegistry.ConfigureBeans(beans =>
            {
                foreach (var c in importComps)
                {
                    if (_parsedComponents.Contains(c))
                    {
                        continue;
                    }

                    _parsedComponents.Add(c);

                    beans.AddComponent(c);
                }
            });

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
