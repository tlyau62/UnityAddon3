using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Aop;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean;
using UnityAddon.Core.Exceptions;
using UnityAddon.Core.DependencyInjection;

namespace UnityAddon.Core.BeanBuildStrategies
{
    [Component]
    public class BeanAutowireStrategy : BuilderStrategy
    {
        [Dependency]
        public PropertyFill PropertyFill { get; set; }

        public override void PostBuildUp(ref BuilderContext context)
        {
            PropertyFill.FillAllProperties(context.Existing);

            base.PostBuildUp(ref context);
        }
    }
}
