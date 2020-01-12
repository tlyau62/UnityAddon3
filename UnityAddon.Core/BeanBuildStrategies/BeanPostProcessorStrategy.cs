using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.BeanPostprocessor;

namespace UnityAddon.Core.BeanBuildStrategies
{
    [Component]
    public class BeanPostProcessorStrategy : BuilderStrategy
    {
        [Dependency]
        public BeanPostProcessorLoader BeanPostProcessorLoader { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            if (BeanPostProcessorLoader.Isloaded)
            {
                var bean = context.Existing;

                foreach (var processor in BeanPostProcessorLoader.BeanPostProcessors)
                {
                    processor.PostProcess(bean, context.Name);
                }
            }

            base.PostBuildUp(ref context);
        }
    }
}
