using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;

namespace UnityAddon.Core.Bean
{
    [Component]
    public class ConstructorResolver
    {
        public ConstructorInfo ChooseConstuctor(Type type, IServiceProvider sp)
        {
            var ctors = type.GetConstructors()
                .Where(ctor => ctor.GetParameters().Select(p => p.ParameterType).All(t => sp.CanResolve(t)));
            var maxFill = ctors.Max(ctor => ctor.GetParameters().Count());

            return ctors.Where(ctor => ctor.GetParameters().Count() == maxFill).Single();
        }
    }
}
