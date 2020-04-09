using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition>
    {
        public string Name { get; }
    }

    public class BeanDefinitionCollection : List<IBeanDefinition>, IBeanDefinitionCollection
    {
        public BeanDefinitionCollection()
        {
        }

        public BeanDefinitionCollection(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
