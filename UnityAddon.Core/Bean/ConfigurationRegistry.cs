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
        public ApplicationContext AppContext { get; set; }

        /// <summary>
        /// should be called before appCtx is firstly refreshed 
        /// </summary>
        public void RegisterConfigurations()
        {
            AppContext.ConfigureBeans((config, sp) =>
            {
                var beanMethodDefs = BeanDefContainer.Registrations.Values
                    .SelectMany(holder => holder.GetAll().Where(def => def is MemberConfigurationBeanDefinition))
                    .SelectMany(configBeanDef => ParseBeanMethods(configBeanDef.Type));

                config.AddRange(beanMethodDefs);
            }, ApplicationContextEntryOrder.BeanMethod);
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
