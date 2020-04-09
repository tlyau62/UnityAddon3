using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace UnityAddon.Core.BeanDefinition.Candidate
{
    public class NamespaceFilter : IBeanDefinitionCandidateFilter
    {
        private readonly string[] _namespaces;

        public NamespaceFilter(params string[] namespaces)
        {
            _namespaces = namespaces;
        }

        public bool IsMatch(IBeanDefinition beanDefinition, IConfiguration configuration)
        {
            return _namespaces.Any(ns => beanDefinition.Namespace.StartsWith(ns));
        }
    }
}
