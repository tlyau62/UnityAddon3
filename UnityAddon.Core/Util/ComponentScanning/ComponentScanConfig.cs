using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Util.ComponentScanning
{
    [Configuration]
    public class ComponentScanConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection ComponentScan(UnityAddonTest testcase, [Dependency("Namespaces")] string[] namespaces)
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            var nsmerge = new[] { testcase.GetType().Namespace }.Union(namespaces).ToArray();

            col.AddFromComponentScanner(testcase.GetType().Assembly, nsmerge);

            return col;
        }
    }
}
