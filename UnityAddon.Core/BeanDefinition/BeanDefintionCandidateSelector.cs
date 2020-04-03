using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.BeanDefinition
{
    /// <summary>
    /// Decide which bean definition candidate is finally
    /// registered into the bean container and bean factory.
    /// </summary>
    public class BeanDefintionCandidateSelector
    {
        [Dependency]
        public IConfiguration Configuration { get; set; }

        private string _activeProfiles { get => Configuration["profiles:active"]; }

        public IEnumerable<IBeanDefinition> Select(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            return beanDefinitions.Where(def => !IsFilteredByProfile(def));
        }

        private bool IsFilteredByProfile(IBeanDefinition beanDefinition)
        {
            var profiles = beanDefinition.BeanProfiles;

            if (_activeProfiles != null && profiles.Length > 0)
            {
                var activeProfiles = _activeProfiles.Split(',');

                if (activeProfiles.All(ap => !profiles.Contains(ap)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
