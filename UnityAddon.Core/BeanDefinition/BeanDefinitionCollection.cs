using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition> { }

    public class BeanDefinitionCollection : List<IBeanDefinition>, IBeanDefinitionCollection
    {
        public BeanDefinitionCollection()
        {
            Add(new SimpleBeanDefinition(typeof(IUnityContainer)));
        }
    }
}
