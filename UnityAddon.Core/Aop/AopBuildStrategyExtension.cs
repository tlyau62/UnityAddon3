using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Extension;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.BeanBuildStrategies;

namespace UnityAddon.Core.Aop
{
    [Component]
    public class AopBuildStrategyExtension : UnityContainerExtension
    {
        [Dependency]
        public BeanAopStrategy BeanAopStrategy { get; set; }

        protected override void Initialize()
        {
            Context.Strategies.Add(BeanAopStrategy, UnityBuildStage.Initialization);
        }
    }
}
