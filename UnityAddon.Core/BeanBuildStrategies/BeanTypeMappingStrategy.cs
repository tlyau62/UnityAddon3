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
            if (context.Type.IsGenericType)
            {
                base.PreBuildUp(ref context);
                return;
            }

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

            // TODO: temp solution
            if (context.Type.HasAttribute<ComponentAttribute>(true) && context.Type.HasAttribute<ProfileAttribute>())
            {
                string activeProfiles = (string)ValueProvider.GetValue(typeof(string), "{profiles.active:}");

                if (IsFilteredByProfile(activeProfiles, context.Type.GetAttribute<ProfileAttribute>().Values))
                {
                    context.Existing = null;
                    context.BuildComplete = true;
                    return;
                }
            }

            base.PreBuildUp(ref context);
        }

        public bool IsFilteredByProfile(string activeProfiles, string[] profiles)
        {
            if (activeProfiles != null && profiles.Length > 0)
            {
                var splits = activeProfiles.Split(',');

                if (splits.All(ap => !profiles.Contains(ap)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
