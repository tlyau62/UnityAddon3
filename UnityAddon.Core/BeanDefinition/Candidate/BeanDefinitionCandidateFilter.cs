using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCandidateFilter
    {
        public bool IsMatch(IBeanDefinition beanDefinition, IConfiguration configuration);
    }
}
