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
            if (context.Type.IsGenericType)
            {
                var genericType = context.Type.GetGenericTypeDefinition();

                if (BeanDefinitionContainer.HasBeanDefinition(genericType))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericType, context.Name);
                    var concreteType = beanDef.GetBeanType().MakeGenericType(context.Type.GetGenericArguments());

                    if (context.Type.GetGenericTypeDefinition() != beanDef.GetBeanType())
                    {
                        // type mapping not work for generic type
                        context.Existing = context.Resolve(concreteType, null);
                        context.BuildComplete = true;
                        return;
                    }

                    context.RegistrationType = context.Type = concreteType;

                    if (context.Name != null && !context.Name.StartsWith("#"))
                    {
                        context.Name = null;
                    }


                }
            }
            else
            {
                if (BeanDefinitionContainer.HasBeanDefinition(context.Type))
                {
                    var beanDef = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name);

                    context.RegistrationType = context.Type = beanDef.GetBeanType();

                    if (context.Name != null && !context.Name.StartsWith("#"))
                    {
                        context.Name = null;
                    }
                }
            }



            base.PreBuildUp(ref context);
        }
    }
}
