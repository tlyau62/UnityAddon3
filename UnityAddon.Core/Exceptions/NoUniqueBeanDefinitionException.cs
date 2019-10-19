using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.Exceptions
{
    /// <summary>
    /// Throw if more than 1 bean definition is found.
    /// </summary>
    public class NoUniqueBeanDefinitionException : NoSuchBeanDefinitionException
    {
        public BeanDefinitionHolder BeanDefinitionHolder { get; set; }

        public NoUniqueBeanDefinitionException()
        {
        }

        public NoUniqueBeanDefinitionException(BeanDefinitionHolder beanDefinitionHolder)
        {
            BeanDefinitionHolder = beanDefinitionHolder;
        }

        public NoUniqueBeanDefinitionException(string message) : base(message)
        {
        }
    }
}
