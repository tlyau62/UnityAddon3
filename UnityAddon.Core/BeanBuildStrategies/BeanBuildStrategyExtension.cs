using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Extension;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Handle bean resolving logic.
    /// </summary>
    [Component]
    public class BeanBuildStrategyExtension : UnityContainerExtension
    {
        [Dependency]
        public BeanTypeMappingStrategy BeanTypeMappingStrategy { get; set; }

        [Dependency]
        public BeanDependencyValidatorStrategy BeanDependencyValidatorStrategy { get; set; }

        [Dependency]
        public BeanPostConstructStrategy BeanPostConstructStrategy { get; set; }

        [Dependency]
        public BeanGenericTypeMappingStrategy BeanGenericTypeMappingStrategy { get; set; }

        [Dependency]
        public BeanAopStrategy BeanAopStrategy { get; set; }

        /// <summary>
        /// In order.
        /// For prebuild methods, they are executed according to the add order.
        /// For postbuild methods, they are executed according to the reverse of add order.
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.Add(BeanTypeMappingStrategy, UnityBuildStage.TypeMapping); // 1
            Context.Strategies.Add(BeanGenericTypeMappingStrategy, UnityBuildStage.TypeMapping); // 2
            Context.Strategies.Add(BeanDependencyValidatorStrategy, UnityBuildStage.PreCreation); // 3
            Context.Strategies.Add(BeanAopStrategy, UnityBuildStage.PostInitialization); // 5
            Context.Strategies.Add(BeanPostConstructStrategy, UnityBuildStage.PostInitialization); // 4 (before BeanAopStrategy, so interceptor will not trigget at postconstruct)
        }
    }
}
