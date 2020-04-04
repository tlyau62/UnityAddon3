using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAddon.Core.Exceptions;

namespace UnityAddon.Core.BeanDefinition
{
    /// <summary>
    /// Hold all the bean definition of the same type.
    /// </summary>
    public class BeanDefinitionHolder
    {
        private List<IBeanDefinition> _beanDefinitions = new List<IBeanDefinition>();

        public void Add(params IBeanDefinition[] beanDefinitions)
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
        public IEnumerable<IBeanDefinition> Find(string name = null)
        {
            if (name == null)
            {
                if (_beanDefinitions.Any(def => def.IsPrimary))
                {
                    return _beanDefinitions.Where(def => def.IsPrimary);
                }

                return _beanDefinitions;
            }

            return _beanDefinitions.Where(d => d.BeanQualifiers.Any(q => q == name) || d.BeanName == name);
        }

        /// <summary>
        /// Throw exception if not a single definition is found.
        /// </summary>
        public IBeanDefinition Get(string name = null)
        {
            var results = Find(name);

            if (results.Count() == 0)
            {
                throw new NoSuchBeanDefinitionException();
            }
            else if (results.Count() > 1)
            {
                throw new NoUniqueBeanDefinitionException(this);
            }

            return results.Single();
        }

        public IEnumerable<IBeanDefinition> GetAll()
        {
            return _beanDefinitions;
        }

        public override string ToString()
        {
            return string.Join("\r\n", _beanDefinitions.Select(def => $"- {def}").ToArray());
        }

        public void Remove(params IBeanDefinition[] beanDefinitions)
        {
            foreach (var beanDef in beanDefinitions)
            {
                _beanDefinitions.Remove(beanDef);
            }
        }
    }
}
