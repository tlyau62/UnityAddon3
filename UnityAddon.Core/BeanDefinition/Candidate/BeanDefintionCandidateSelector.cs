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
        private readonly IConfiguration _configuration;

        private readonly IEnumerable<IBeanDefinitionCandidateFilter> _includeFilters;

        private readonly IEnumerable<IBeanDefinitionCandidateFilter> _excludeFilters;

        public BeanDefintionCandidateSelector(BeanDefintionCandidateSelectorOption option, IConfiguration configuration)
        {
            _includeFilters = option.IncludeFilters;
            _excludeFilters = option.ExcludeFilters;
            _configuration = configuration;
        }

        public IEnumerable<IBeanDefinition> Select(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            return beanDefinitions.Where(def => Filter(def));
        }

        public bool Filter(IBeanDefinition definition)
        {
            if (_excludeFilters.Any(f => f.IsMatch(definition, _configuration)))
            {
                return false;
            }

            if (_includeFilters.Count() == 0 || _includeFilters.Any(f => f.IsMatch(definition, _configuration)))
            {
                return true;
            }

            return false;
        }
    }
}
