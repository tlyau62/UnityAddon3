using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Reflection;
using UnityAddon.Core.Value;

namespace UnityAddon.Core.BeanBuildStrategies
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

        [Dependency]
        public ValueProvider ValueProvider { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (BeanDefinitionContainer.HasBeanDefinition(context.Type, context.Name))
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
            else if (context.Type.IsGenericType && BeanDefinitionContainer.HasBeanDefinition(context.Type.GetGenericTypeDefinition(), context.Name))
            {
                var genericTypeDef = context.Type.GetGenericTypeDefinition();

                if (context.Name == null || !context.Name.StartsWith("#"))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
                    var makeGenericType = beanDef.GetBeanType().MakeGenericType(context.Type.GetGenericArguments());

                    if (makeGenericType != context.Type || context.Name != beanDef.GetBeanName())
                    {
                        // use resolve type params to make the generic type from bean def
                        context.Existing = context.Resolve(makeGenericType, beanDef.GetBeanName());
                        context.BuildComplete = true;
                        return;
                    }
                }
            }

            base.PreBuildUp(ref context);
        }

    }
}
