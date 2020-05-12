using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Configs
{
    [Configuration]
    public class ComponentScanTestConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection ComponentScan([Dependency("TestCase")] Type testcase, [Dependency("Namespaces")] string[] namespaces)
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(testcase.Assembly, new[] { testcase.Namespace }.Union(namespaces).ToArray());

            return col;
        }
    }
}
