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
    /// <summary>
    /// Handle generic type resolving logic.
    /// </summary>
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
                    var isConcreteType = !beanDef.GetBeanType().IsGenericType || !beanDef.GetBeanType().ContainsGenericParameters;

                    var concreteType = isConcreteType ? beanDef.GetBeanType() : beanDef.GetBeanType().MakeGenericType(context.Type.GetGenericArguments());

                    if (concreteType != context.Type || context.Name != beanDef.GetBeanName())
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
