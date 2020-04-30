using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Util.ComponentScanning
{
    public static class ComponentScannerBeanDefinitionCollectionExt
    {
        public static IBeanDefinitionCollection AddFromComponentScanner(this IBeanDefinitionCollection beanDefinitions, Func<ComponentScanner, IBeanDefinitionCollection> func, ComponentScannerOption option)
        {
            return beanDefinitions.AddFromExisting(func(new ComponentScanner(option)));
        }

        public static IBeanDefinitionCollection AddFromComponentScanner(this IBeanDefinitionCollection beanDefinitions, Func<ComponentScanner, IBeanDefinitionCollection> func)
        {
            return beanDefinitions.AddFromComponentScanner(func, new ComponentScannerOption());
        }
    }
}
