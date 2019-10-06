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
    /// For mapping generic type.
    /// Redirecting the mapping type cannot invoke any invoked factory method, i.e.
    /// context.RegistrationType = context.Type = beanDef.GetBeanType();
    /// Thus, this approach does not work for the singleton bean.
    /// 
    /// The solution is to resolve the generic type again, i.e.
    /// context.Resolve(concreteType, context.Name)
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
