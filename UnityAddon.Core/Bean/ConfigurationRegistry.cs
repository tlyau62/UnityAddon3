using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.Context;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Bean
{
    public class ConfigurationRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefContainer { get; set; }

        [Dependency]
        public IServiceRegistry ServiceRegistry { get; set; }

        private readonly HashSet<IBeanDefinition> _parsedConfigs = new HashSet<IBeanDefinition>();

        public void Refresh()
        {
            var beanMethodDefs = BeanDefContainer.Registrations.Values
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
                .SelectMany(configBeanDef => ParseBeanMethods(configBeanDef.Type))
                .ToArray();

            if (beanMethodDefs.Length == 0)
            {
                return;
            }

            ServiceRegistry.ConfigureBeans(config => config.AddRange(beanMethodDefs));
            Refresh();
        }

        public IBeanDefinitionCollection ParseBeanMethods(Type configType)
        {
            var defCol = new BeanDefinitionCollection();

            defCol.AddRange(MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(configType)
                .SelectMany(beanMethod =>
                {
                    var def = new MemberMethodBeanDefinition(beanMethod);

                    return new[] { def, new MemberMethodFactoryBeanDefinition(def) };
                })
                .ToArray());

            return defCol;
        }
    }
}
