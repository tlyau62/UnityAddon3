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
    [Component]
    public class BeanTypeMappingStrategy : BuilderStrategy
    {
        [Dependency]
        public IBeanDefinitionContainer BeanDefinitionContainer { get; set; }

        public override void PreBuildUp(ref BuilderContext context)
        {
            object resolved = null;
            bool isResolved = false;
            var exists = false;
            var container = context.Container;

            if (BeanDefinitionContainer.HasBeanDefinition(context.Type, context.Name))
            {
                exists = true;

                var beanDef = BeanDefinitionContainer.GetBeanDefinition(context.Type, context.Name);

                if (context.Type != beanDef.Type || context.Name != beanDef.Name)
                {
                    resolved = context.Container.Resolve(beanDef.Type, beanDef.Name);
                    isResolved = true;
                }
            }
            else if (context.Type.IsGenericType && BeanDefinitionContainer.HasBeanDefinition(context.Type.GetGenericTypeDefinition(), context.Name))
            {
                exists = true;

                var genericTypeDef = context.Type.GetGenericTypeDefinition();
                var beanDef = BeanDefinitionContainer.GetBeanDefinition(genericTypeDef, context.Name);
                var makeGenericType = beanDef.Type.MakeGenericType(context.Type.GetGenericArguments()); // use resolve type params to make the generic type from bean def

                if (context.Type.GetGenericTypeDefinition() != beanDef.Type || context.Name != beanDef.Name)
                {
                    resolved = context.Container.Resolve(makeGenericType, beanDef.Name);
                    isResolved = true;
                }
            }
            else if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                exists = true;

                var beanDefs = BeanDefinitionContainer.GetAllBeanDefinitions(context.Type.GetGenericArguments()[0]);
                var array = (object[])Array.CreateInstance(context.Type.GetGenericArguments()[0], beanDefs.Count());

                Array.Copy(beanDefs.Select(def => container.Resolve(def.Type, def.Name)).ToArray(), array, beanDefs.Count());

                resolved = array;
                isResolved = true;
            }

            if (isResolved || !exists)
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
