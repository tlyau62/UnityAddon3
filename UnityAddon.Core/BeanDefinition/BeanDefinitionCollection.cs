using System;
using System.Collections.Generic;
using System.Text;

namespace UnityAddon.Core.BeanDefinition
{
    public interface IBeanDefinitionCollection : IList<IBeanDefinition> { }

    public class BeanDefinitionCollection : List<IBeanDefinition>, IBeanDefinitionCollection
    {
    }
}
