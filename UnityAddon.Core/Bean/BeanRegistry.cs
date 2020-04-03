using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Decide which bean definition candidate is finally
    /// registered into the bean container and bean factory.
    /// </summary>
    [Component]
    public class BeanRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanFactory BeanFactory { get; set; }

        [Value("{profiles.active:}")]
        public string ActiveProfiles { get; set; }

        public void Register(IBeanDefinition beanDefinition, IUnityContainer container)
        {
            if (IsFilteredByProfile(beanDefinition))
            {
                return;
            }

            if (beanDefinition.RequireAssignableTypes)
            {
                BeanDefinitionContainer.RegisterBeanDefinitionForAllTypes(beanDefinition);
            }
            else
            {
                BeanDefinitionContainer.RegisterBeanDefinition(beanDefinition);
            }

            if (beanDefinition.RequireFactory)
            {
                BeanFactory.CreateFactory((dynamic)beanDefinition, container);
            }
        }

        private bool IsFilteredByProfile(IBeanDefinition beanDefinition)
        {
            var profiles = beanDefinition.BeanProfiles;

            if (ActiveProfiles != null && profiles.Length > 0)
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
