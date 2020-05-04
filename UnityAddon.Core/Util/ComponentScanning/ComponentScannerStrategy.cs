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
            return new BeanDefinitionCollection() { new MemberComponentBeanDefinition(type) };
        }

        public bool IsMatch(Type type)
        {
            return true;
        }
    }

    [Order(Ordered.LOWEST_PRECEDENCE - 1)]
    public class ConfigurationScannerStrategy : IComponentScannerStrategy
    {
        public IBeanDefinitionCollection Create(Type type)
        {
            var defCol = new BeanDefinitionCollection() { new MemberConfigurationBeanDefinition(type) };

            defCol.AddRange(Parse(type));

            return defCol;
        }

        public IBeanDefinitionCollection Parse(Type config)
        {
            var defCol = new BeanDefinitionCollection();

            defCol.AddRange(MethodSelector.GetAllMethodsByAttribute<BeanAttribute>(config)
                .SelectMany(beanMethod =>
                {
                    var def = new MemberMethodBeanDefinition(beanMethod);

                    return new[] { def, new MemberMethodFactoryBeanDefinition(def) };
                })
                .ToArray());

            return defCol;
        }

        public bool IsMatch(Type type)
        {
            return type.HasAttribute<ConfigurationAttribute>();
        }
    }
}
