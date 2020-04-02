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
        public BeanAutowireStrategy BeanAutowireStrategy { get; set; }

        [Dependency]
        public BeanPostConstructStrategy BeanPostConstructStrategy { get; set; }

        /// <summary>
        /// In order.
        /// For prebuild methods, they are executed according to the add order.
        /// For postbuild methods, they are executed according to the reverse of add order.
        /// </summary>
        protected override void Initialize()
        {
            Context.Strategies.Add(BeanTypeMappingStrategy, UnityBuildStage.TypeMapping);
            Context.Strategies.Add(BeanPostConstructStrategy, UnityBuildStage.PostInitialization);
            Context.Strategies.Add(BeanAutowireStrategy, UnityBuildStage.PostInitialization);
        }
    }
}
