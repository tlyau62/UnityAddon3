using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Bean
{
    public class BeanDefinitionHolder
    {
        private List<AbstractBeanDefinition> _beanDefinitions = new List<AbstractBeanDefinition>();

        public void Add(params AbstractBeanDefinition[] beanDefinitions)
        {
            if (beanDefinitions.Length == 0)
            {
                throw new InvalidOperationException();
            }

            _beanDefinitions.AddRange(beanDefinitions);
        }

        /// <summary>
        /// Return true if any bean definition is found.
        /// </summary>
        public bool Exist(string name)
        {
            return Find(name).Length > 0;
        }

        /// <summary>
        /// Find a bean definition by qualifers.
        /// If not found, find by bean name as fallback.
        /// </summary>
        public AbstractBeanDefinition[] Find(string name)
        {
            if (name == null)
            {
                return _beanDefinitions.ToArray();
            }

            var defs = _beanDefinitions.Where(d => d.GetBeanQualifiers().Any(q => q == name));

            if (defs.Count() == 0)
            {
                defs = _beanDefinitions.Where(d => d.GetBeanName() == name);
            }

            return defs.ToArray();
        }

        /// <summary>
        /// Throw exception if not a single definition is found.
        /// </summary>
        public AbstractBeanDefinition Get(string name)
        {
            return Find(name).Single();
        }
    }
}
