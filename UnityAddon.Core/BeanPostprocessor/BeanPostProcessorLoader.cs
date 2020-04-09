using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using UnityAddon.Core.Bean;
using System.Linq;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.BeanPostprocessor
{
    /// <summary>
    /// Load all BeanPostProcessor before application beans are instantiated.
    /// </summary>
    public class BeanPostProcessorLoader
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public IEnumerable<IBeanPostProcessor> BeanPostProcessors { get; set; }

        public bool Isloaded => BeanPostProcessors != null;

        /// <summary>
        /// Instantiate all bean processors.
        /// ToList is required for eagerly loading.
        /// The execution order of the bean processors will be sorted by the [Order] attribute.
        /// </summary>
        /// <returns>All beanPostProcessors</returns>
        public IEnumerable<IBeanPostProcessor> LoadBeanPostProcessors()
        {
            throw new NotImplementedException();

            //return BeanPostProcessors = BeanDefinitionContainer
            //    .GetAllBeanDefinitions(typeof(IBeanPostProcessor))
            //    .Where(def => !def.BeanType.IsGenericType || !def.BeanType.ContainsGenericParameters)
            //    .Select(def => ContainerRegistry.Resolve(def.BeanType, def.BeanName))
            //    .OrderBy(bean => bean.GetType().GetOrder())
            //    .Cast<IBeanPostProcessor>()
            //    .ToList();
        }


    }
}
