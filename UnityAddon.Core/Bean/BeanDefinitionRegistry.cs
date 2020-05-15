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
    public class BeanDefinitionRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefContainer { get; set; }

        [Dependency]
        public ApplicationContext AppContext { get; set; }

        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        private readonly HashSet<IBeanDefinition> _parsedCols = new HashSet<IBeanDefinition>();

        public void Refresh()
        {
            var hasNew = false;

            foreach (var colDef in BeanDefContainer.GetAllBeanDefinitions(typeof(IBeanDefinitionCollection)))
            {
                if (_parsedCols.Contains(colDef))
                {
                    continue;
                }

                _parsedCols.Add(colDef);

                AppContext.ConfigureBeans(config => config.AddFromExisting(Sp.GetRequiredService<IBeanDefinitionCollection>(colDef.Name)));

                hasNew = true;
            }

            if (hasNew)
            {
                Refresh();
            }
        }
    }
}
