﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Util.ComponentScanning;

namespace UnityAddon.Core.Bean
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

            beanDefinitions.AddFromExisting(new ComponentScanner(opt).ScanAssembly(assembly, namespaces));

            return beanDefinitions;
        }
    }
}
