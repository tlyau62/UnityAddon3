using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.BeanDefinition
{
    public class BeanDefintionCandidateSelectorBuilder
    {
        private readonly List<IBeanDefinitionCandidateFilter> _includeFilters;

        private readonly List<IBeanDefinitionCandidateFilter> _excludeFilters;

        public BeanDefintionCandidateSelectorBuilder()
        {
            _includeFilters = new List<IBeanDefinitionCandidateFilter>();
            _excludeFilters = new List<IBeanDefinitionCandidateFilter>();
            AddDefaultFilters();
        }

        protected void AddDefaultFilters()
        {
            _excludeFilters.Add(new InactiveProfileFilter());
        }

        public BeanDefintionCandidateSelectorBuilder AddIncludeFilter(params IBeanDefinitionCandidateFilter[] includeFilters)
        {
            _includeFilters.AddRange(includeFilters);

            return this;
        }

        public BeanDefintionCandidateSelectorBuilder AddExcludeFilter(params IBeanDefinitionCandidateFilter[] excludeFilters)
        {
            _excludeFilters.AddRange(excludeFilters);

            return this;
        }

        public BeanDefintionCandidateSelector Build(IConfiguration configuration)
        {
            return new BeanDefintionCandidateSelector(_includeFilters, _excludeFilters, configuration);
        }
    }
}
