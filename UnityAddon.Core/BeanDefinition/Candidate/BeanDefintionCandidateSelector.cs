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
        private readonly List<IBeanDefinitionCandidateFilter> _includeFilters = new List<IBeanDefinitionCandidateFilter>();

        private readonly List<IBeanDefinitionCandidateFilter> _excludeFilters = new List<IBeanDefinitionCandidateFilter>();

        public BeanDefintionCandidateSelector()
        {
            _excludeFilters.Add(new InactiveProfileFilter());
        }

        [Dependency]
        public IConfiguration Configuration { get; set; }

        public IEnumerable<IBeanDefinition> Select(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            return beanDefinitions.Where(def => Filter(def));
        }

        public bool Filter(IBeanDefinition definition)
        {
            if (_excludeFilters.Any(f => f.IsMatch(definition, Configuration)))
            {
                return false;
            }

            if (_includeFilters.Count() == 0 || _includeFilters.Any(f => f.IsMatch(definition, Configuration)))
            {
                return true;
            }

            return false;
        }
    }
}
