using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Component
{
    public interface IComponentScannerStrategy
    {
        bool IsMatch(Type type);

        IBeanDefinitionCollection Create(Type type);
    }

    [Order(Ordered.LOWEST_PRECEDENCE)]
    public class DefaultComponentScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinitionCollection Create(Type type)
        {
            return new BeanDefinitionCollection() { new TypeBeanDefinition(type) };
        }

        public bool IsMatch(Type type)
        {
            return true;
        }
    }
}
