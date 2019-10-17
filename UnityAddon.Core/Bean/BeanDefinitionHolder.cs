using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAddon.Core.Bean
{
    /// <summary>
    /// Hold all the bean definition of the same type.
    /// </summary>
    public class BeanDefinitionHolder
    {
        private List<AbstractBeanDefinition> _beanDefinitions = new List<AbstractBeanDefinition>();

        public void Add(params AbstractBeanDefinition[] beanDefinitions)
        {
            if (beanDefinitions.Length == 0)
            {
                throw new InvalidOperationException("Bean definition holder must be added with at least one bean definition.");
            }

            _beanDefinitions.AddRange(beanDefinitions);
        }

        /// <summary>
        /// Return true if any bean definition is found.
        /// </summary>
        public bool Exist(string name)
        {
            return Find(name).Count() > 0;
        }

        /// <summary>
        /// Find a bean definition by qualifers.
        /// If not found, find by bean name as fallback.
        /// </summary>
        public IEnumerable<AbstractBeanDefinition> Find(string name = null)
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

            return defs;
        }

        /// <summary>
        /// Throw exception if not a single definition is found.
        /// </summary>
        public AbstractBeanDefinition Get(string name = null)
        {
            var results = Find(name);

            if (name == null)
            {
                var primaryDefs = results.Where(def => def.IsPrimary());

                if (primaryDefs.Count() > 0)
                {
                    results = primaryDefs;
                }
            }

            return results.Single();
        }

        public IEnumerable<AbstractBeanDefinition> GetAll()
        {
            return _beanDefinitions;
        }
    }
}
