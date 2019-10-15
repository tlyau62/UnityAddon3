using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Attributes;

namespace UnityAddon.Bean
{
    [Component]
    public class BeanDefinitionRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanFactory BeanFactory { get; set; }

        [Value("{profiles.active:}")]
        public string ActiveProfiles { get; set; }

        public void Register(AbstractBeanDefinition beanDefinition)
        {
            if (IsFilteredByProfile(beanDefinition))
            {
                return;
            }

            BeanDefinitionContainer.RegisterBeanDefinition(beanDefinition);
            BeanFactory.CreateFactory((dynamic)beanDefinition);
        }

        private bool IsFilteredByProfile(AbstractBeanDefinition beanDefinition)
        {
            var profiles = beanDefinition.GetBeanProfiles();

            if (!string.IsNullOrEmpty(ActiveProfiles) && profiles.Length > 0)
            {
                var activeProfiles = ActiveProfiles.Split(',');

                if (activeProfiles.All(ap => !profiles.Contains(ap)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
