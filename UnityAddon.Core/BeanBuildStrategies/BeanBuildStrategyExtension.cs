﻿using System;
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
        /// In order
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.Add(BeanTypeMappingStrategy, UnityBuildStage.TypeMapping);
            Context.Strategies.Add(BeanGenericTypeMappingStrategy, UnityBuildStage.TypeMapping);
            Context.Strategies.Add(BeanDependencyValidatorStrategy, UnityBuildStage.PreCreation);
            Context.Strategies.Add(BeanPostConstructStrategy, UnityBuildStage.Initialization);
            Context.Strategies.Add(BeanAopStrategy, UnityBuildStage.PostInitialization);
        }
    }
}