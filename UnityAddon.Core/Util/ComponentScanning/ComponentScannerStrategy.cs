using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.BeanDefinition.MemberBean;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.Util.ComponentScanning
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
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            if (type.HasAttribute<ConfigurationAttribute>())
            {
                col.AddConfiguration(type);
            }
            else
            {
                col.AddComponent(type);
            }

            return col;
        }

        public bool IsMatch(Type type)
        {
            return true;
        }
    }

}
