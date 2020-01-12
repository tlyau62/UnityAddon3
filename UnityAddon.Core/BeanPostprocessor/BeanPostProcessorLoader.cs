using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using System.Linq;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanPostprocessor
{
    /// <summary>
    /// Load all BeanPostProcessor before application beans are instantiated.
    /// </summary>
    public class BeanPostProcessorLoader
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        public IEnumerable<IBeanPostProcessor> BeanPostProcessors { get; set; }

        public bool Isloaded => BeanPostProcessors != null;

        /// <summary>
        /// Instantiate all bean processors.
        /// ToList is required for eagerly loading.
        /// TODO: The execution order of the bean processors will be sorted by the [Order] attribute.
        /// </summary>
        /// <returns>All beanPostProcessors</returns>
        public IEnumerable<IBeanPostProcessor> LoadBeanPostProcessors()
        {
            return BeanPostProcessors = BeanDefinitionContainer
                .GetAllBeanDefinitions(typeof(IBeanPostProcessor))
                .Where(def => !def.GetBeanType().IsGenericType || !def.GetBeanType().ContainsGenericParameters)
                .Select(def => ContainerRegistry.Resolve(def.GetBeanType(), def.GetBeanName()))
                .Cast<IBeanPostProcessor>()
                .ToList();
        }
    }
}
