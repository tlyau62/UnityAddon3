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
        public virtual IBeanDefinitionCollection ComponentScan([Dependency("TestCase")] Type testcase, [Dependency("Namespaces")] string[] namespaces)
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();
            var nsmerge = new[] { testcase.Namespace }.Union(namespaces).ToArray();

            col.AddFromComponentScanner(testcase.Assembly, nsmerge);

            return col;
        }
    }
}
