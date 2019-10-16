using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using Unity.Builder;
using Unity.Strategies;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Reflection;

namespace UnityAddon.Core.BeanBuildStrategies
{
    /// <summary>
    /// Invoke postconstruct
    /// </summary>
    [Component]
    public class BeanPostConstructStrategy : BuilderStrategy
    {
        public override void PostBuildUp(ref BuilderContext context)
        {
            var postConstructors = MethodSelector.GetAllMethodsByAttribute<PostConstructAttribute>(context.Type);

            foreach (var pc in postConstructors)
            {
                if (pc.GetParameters().Length > 0 || pc.ReturnType != typeof(void))
                {
                    throw new InvalidOperationException("no-arg, void");
                }

                pc.Invoke(context.Existing, new object[0]);
            }

            base.PostBuildUp(ref context);
        }
    }
}
