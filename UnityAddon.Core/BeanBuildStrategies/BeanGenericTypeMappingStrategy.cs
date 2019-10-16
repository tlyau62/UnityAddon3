using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.BeanBuildStrategies
{
    [Component]
    public class BeanGenericTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (!context.Type.IsGenericType)
            {
                base.PreBuildUp(ref context);
                return;
            }

            var genericTypeDef = context.Type.GetGenericTypeDefinition();

            if (BeanDefinitionContainer.HasBeanDefinition(genericTypeDef))
            {
                if (context.Name == null || !context.Name.StartsWith("#"))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
                    var concreteType = beanDef.GetBeanType().MakeGenericType(context.Type.GetGenericArguments());

                    if (genericTypeDef != beanDef.GetBeanType() || context.Name != beanDef.GetBeanName())
                    {
                        // use resolve type params to make the generic type from bean def
                        context.Existing = context.Resolve(concreteType, beanDef.GetBeanName());
                        context.BuildComplete = true;
                        return;
                    }
                }
            }

            base.PreBuildUp(ref context);
        }
    }
}
