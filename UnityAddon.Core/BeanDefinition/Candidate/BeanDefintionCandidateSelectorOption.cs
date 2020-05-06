using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.BeanDefinition
{
    public class BeanDefintionCandidateSelectorOption
    {
        private readonly List<IBeanDefinitionCandidateFilter> _includeFilters;

        private readonly List<IBeanDefinitionCandidateFilter> _excludeFilters;

        public IEnumerable<IBeanDefinitionCandidateFilter> IncludeFilters => _includeFilters;

        public IEnumerable<IBeanDefinitionCandidateFilter> ExcludeFilters => _excludeFilters;

        public BeanDefintionCandidateSelectorOption()
        {
            _includeFilters = new List<IBeanDefinitionCandidateFilter>();
            _excludeFilters = new List<IBeanDefinitionCandidateFilter>();
            AddDefaultFilters();
        }

        protected void AddDefaultFilters()
        {
            _excludeFilters.Add(new InactiveProfileFilter());
        }

        public BeanDefintionCandidateSelectorOption AddIncludeFilter(params IBeanDefinitionCandidateFilter[] includeFilters)
        {
            _includeFilters.AddRange(includeFilters);

            return this;
        }

        public BeanDefintionCandidateSelectorOption AddExcludeFilter(params IBeanDefinitionCandidateFilter[] excludeFilters)
        {
            _excludeFilters.AddRange(excludeFilters);

            return this;
        }
    }
}
