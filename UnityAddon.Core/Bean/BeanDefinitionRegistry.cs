using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Decide which bean definition candidate is finally
    /// registered into the bean container and bean factory.
    /// </summary>
    [Component]
    public class BeanDefinitionRegistry
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public BeanFactory BeanFactory { get; set; }

        [Value("{profiles.active:}")]
        public string ActiveProfiles { get; set; }

        [OptionalDependency]
        public ComponentScanNamespaceExcludeFilter ComponentScanNamespaceExcludeFilter { get; set; }

        public void Register(AbstractBeanDefinition beanDefinition)
        {
            if (IsFilteredByProfile(beanDefinition))
            {
                return;
            }

            if (ComponentScanNamespaceExcludeFilter != null && ComponentScanNamespaceExcludeFilter.Values.Any(filter => new Regex(filter).IsMatch(beanDefinition.Namespace)))
            {
                return;
            }

            BeanDefinitionContainer.RegisterBeanDefinition(beanDefinition);
            BeanFactory.CreateFactory((dynamic)beanDefinition);
        }

        private bool IsFilteredByProfile(AbstractBeanDefinition beanDefinition)
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
