using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace UnityAddon.Core.BeanDefinition
{
    public class InactiveProfileFilter : IBeanDefinitionCandidateFilter
    {
        public bool IsMatch(IBeanDefinition beanDefinition, IConfiguration configuration)
        {
            var profiles = beanDefinition.Profiles;
            var activeProfiles = (configuration["profiles:active"] ?? "").Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p));

            if (profiles.Length > 0 && !profiles.Any(p => activeProfiles.Contains(p)))
            {
                return true;
            }

            return false;
        }
    }
}
