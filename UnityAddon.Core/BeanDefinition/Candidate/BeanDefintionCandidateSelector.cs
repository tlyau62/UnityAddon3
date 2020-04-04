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
        public IConfiguration _configuration;

        private IEnumerable<IBeanDefinitionCandidateFilter> _includeFilters;

        private IEnumerable<IBeanDefinitionCandidateFilter> _excludeFilters;

        public BeanDefintionCandidateSelector(IEnumerable<IBeanDefinitionCandidateFilter> includeFilters, IEnumerable<IBeanDefinitionCandidateFilter> excludeFilters, IConfiguration configuration)
        {
            _includeFilters = includeFilters;
            _excludeFilters = excludeFilters;
            _configuration = configuration;
        }

        public IEnumerable<IBeanDefinition> Select(IEnumerable<IBeanDefinition> beanDefinitions)
        {
            return beanDefinitions.Where(def =>
            {
                if (_excludeFilters.Any(f => f.IsMatch(def, _configuration)))
                {
                    return false;
                }

                if (_includeFilters.Count() == 0 || _includeFilters.Any(f => f.IsMatch(def, _configuration)))
                {
                    return true;
                }

                return false;
            });
        }
    }
}
