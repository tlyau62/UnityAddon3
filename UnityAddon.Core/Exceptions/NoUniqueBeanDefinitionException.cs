using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Throw if more than 1 bean definition is found.
    /// </summary>
    public class NoUniqueBeanDefinitionException : NoSuchBeanDefinitionException
    {
        // TODO: Should not put here
        public BeanDefinitionHolder BeanDefinitionHolder { get; set; }

        public NoUniqueBeanDefinitionException(BeanDefinitionHolder beanDefinitionHolder)
        {
            BeanDefinitionHolder = beanDefinitionHolder;
        }

        public NoUniqueBeanDefinitionException(string message) : base(message)
        {
        }
    }
}
