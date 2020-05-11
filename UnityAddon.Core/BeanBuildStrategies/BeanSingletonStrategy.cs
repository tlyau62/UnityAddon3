using Castle.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanDefinition;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Make sure the singleon bean is resolved from the application container
    /// </summary>
    [Component]
    public class BeanSingletonStrategy : BuilderStrategy
    {
        [Dependency]
        public IUnityAddonSP Sp { get; set; }

        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            var appContainer = Sp.UnityContainer;
            IBeanDefinition beanDef = null;

            if (BeanDefinitionContainer.HasBeanDefinition(context.Type, context.Name))
            {
                beanDef = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name);
            }
            else if (context.Type.IsGenericType && BeanDefinitionContainer.HasBeanDefinition(context.Type.GetGenericTypeDefinition(), context.Name))
            {
                var genericTypeDef = context.Type.GetGenericTypeDefinition();
                beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
            }

            if (beanDef != null && beanDef.Scope is SingletonLifetimeManager && context.Container != appContainer)
            {
                context.Existing = appContainer.Resolve(context.Type, context.Name);
                context.BuildComplete = true;
                return;
            }

            base.PreBuildUp(ref context);
        }
    }
}
