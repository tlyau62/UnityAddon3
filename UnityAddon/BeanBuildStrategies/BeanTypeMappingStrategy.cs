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
    /// Map supertype to implementation type
    /// </summary>
    [Component]
    public class BeanTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (BeanDefinitionContainer.HasBeanDefinition(context.Type) && (context.Name == null || (context.Name != null && !context.Name.StartsWith("#")))) // bad
            {
                context.RegistrationType = context.Type = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name).GetBeanType(); // redirect to factory with unity cache
                context.Name = null; // factory has no name
            }

            base.PreBuildUp(ref context);
        }
    }
}
