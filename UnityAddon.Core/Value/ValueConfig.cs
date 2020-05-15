using System;
using System.Collections.Generic;
using System.Text;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.BeanDefinition;

namespace UnityAddon.Core.Value
{
    public class ValueConfig : DependencyResolverConfig
    {
        [Bean]
        public virtual IBeanDefinitionCollection ValueBeans()
        {
            IBeanDefinitionCollection defCol = new BeanDefinitionCollection();

            defCol.AddSingleton<ValueProvider, ValueProvider>();
            defCol.AddSingleton<ConfigBracketParser, ConfigBracketParser>();

            return defCol;
        }

        [Bean]
        public override DependencyResolverOption DependencyResolverOption()
        {
            var option = new DependencyResolverOption();

            option.AddResolveStrategy<ValueAttribute>((type, attr, sp) =>
                sp.GetRequiredService<ValueProvider>().GetValue(type, attr.Value));

            return option;
        }
    }
}
