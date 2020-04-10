using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Map supertype to implementation type.
    /// #: special name used in resolving a special bean factory.
    /// </summary>
    [Component]
    public class BeanTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        /// <summary>
        /// TODO: This function has too many logics.
        /// </summary>
        public override void PreBuildUp(ref BuilderContext context)
        {
            object resolved = null;

            if (BeanDefinitionContainer.HasBeanDefinition(context.Type, context.Name))
            {
                if (context.Name == null || !context.Name.StartsWith("#"))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name);

                    if (context.Type != beanDef.BeanType || context.Name != beanDef.BeanName)
                    {
                        resolved = context.Container.ResolveUA(beanDef.BeanType, beanDef.BeanName);
                    }
                }
            }
            else if (context.Type.IsGenericType && BeanDefinitionContainer.HasBeanDefinition(context.Type.GetGenericTypeDefinition(), context.Name))
            {
                if (context.Name == null || !context.Name.StartsWith("#"))
                {
                    var genericTypeDef = context.Type.GetGenericTypeDefinition();
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
                    var makeGenericType = beanDef.BeanType.MakeGenericType(context.Type.GetGenericArguments()); // // use resolve type params to make the generic type from bean def

                    if (makeGenericType != context.Type || context.Name != beanDef.BeanName)
                    {
                        resolved = context.Container.ResolveUA(makeGenericType, beanDef.BeanName);
                    }
                }
            }

            if (resolved != null)
            {
                context.Existing = resolved;
                context.BuildComplete = true;
            }
            else
            {
                base.PreBuildUp(ref context);
            }
        }

    }
}
