using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;

namespace UnityAddon.Core.BeanPostprocessor
{
    [Component]
    public class BeanPostProcessorStrategy : BuilderStrategy
    {
        [Dependency]
        public IContainerRegistry ContainerRegistry { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            if (!ContainerRegistry.IsRegistered<IEnumerable<IBeanPostProcessor>>())
            {
                base.PostBuildUp(ref context);
                return;
            }

            var pcs = ContainerRegistry.Resolve<IEnumerable<IBeanPostProcessor>>();

            var bean = context.Existing;

            foreach (var processor in pcs)
            {
                processor.PostProcess(bean, context.Name);
            }

            base.PostBuildUp(ref context);
        }
    }
}
