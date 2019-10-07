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
    [Component]
    public class BeanGenericTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            base.PreBuildUp(ref context);
            return;
            if (!context.Type.IsGenericType)
            {
                base.PreBuildUp(ref context);
                return;
            }

            var genericTypeDef = context.Type.GetGenericTypeDefinition();

            if (BeanDefinitionContainer.HasBeanDefinition(genericTypeDef))
            {
                var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
                var concreteType = beanDef.GetBeanType().MakeGenericType(context.Type.GetGenericArguments());

                if (context.Name != null && !context.Name.StartsWith("#"))
                {
                    context.Name = null;
                }

                if (beanDef.GetBeanType() != genericTypeDef)
                {
                    // use resolve type params to make the generic type from bean def
                    context.Existing = context.Resolve(concreteType, context.Name);
                    context.BuildComplete = true;
                    return;
                }
            }

            base.PreBuildUp(ref context);
        }
    }
}
