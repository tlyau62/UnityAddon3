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
        public virtual IBeanDefinitionCollection ComponentScan([Dependency("csconfig_testcase")] Type testcase, [OptionalDependency("csconfig_namespaces")] string[] extranamespaces)
        {
            IBeanDefinitionCollection col = new BeanDefinitionCollection();

            col.AddFromComponentScanner(testcase.Assembly, new[] { testcase.Namespace }.Union(extranamespaces).ToArray());

            return col;
        }
    }
}
