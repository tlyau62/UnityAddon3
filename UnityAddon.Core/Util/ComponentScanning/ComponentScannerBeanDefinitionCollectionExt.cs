using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Util.ComponentScanning
{
    public static class ComponentScannerBeanDefinitionCollectionExt
    {
        public static IBeanDefinitionCollection AddFromComponentScanner(this IBeanDefinitionCollection beanDefinitions, Assembly assembly, params string[] namespaces)
        {
            return beanDefinitions.AddFromComponentScanner(c => { }, assembly, namespaces);
        }

        public static IBeanDefinitionCollection AddFromComponentScanner(this IBeanDefinitionCollection beanDefinitions, Action<ComponentScannerOption> config, Assembly assembly, params string[] namespaces)
        {
            var opt = new ComponentScannerOption();

            config(opt);

            return beanDefinitions.AddFromExisting(new ComponentScanner(opt).ScanAssembly(assembly, namespaces));
        }
    }
}
