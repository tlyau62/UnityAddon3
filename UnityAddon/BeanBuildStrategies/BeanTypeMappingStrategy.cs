using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Attributes;
using UnityAddon.Bean;

namespace UnityAddon.BeanBuildStrategies
{
    /// <summary>
    /// Map supertype to implementation type.
    /// #: special name used in resolving a special bean factory.
    /// Typically, all bean factory is registered without a name.
    /// </summary>
    [Component]
    public class BeanTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (BeanDefinitionContainer.HasBeanDefinition(context.Type))
            {
                if (context.Name == null || !context.Name.StartsWith("#"))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name);

                    if (context.Type != beanDef.GetBeanType() || context.Name != beanDef.GetBeanName())
                    {
                        context.Existing = context.Resolve(beanDef.GetBeanType(), beanDef.GetBeanName());
                        context.BuildComplete = true;
                        return;
                    }
                }
            }

            base.PreBuildUp(ref context);
        }
    }
}
