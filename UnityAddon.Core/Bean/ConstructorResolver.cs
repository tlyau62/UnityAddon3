using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.DependencyInjection;

namespace UnityAddon.Core.Bean
{
    public class ConstructorResolver
    {
        [Dependency]
        public ParameterFill ParameterFill { get; set; }

        public ConstructorInfo ChooseConstuctor(Type type, IServiceProvider sp)
        {
            var ctors = type.GetConstructors()
                .Where(ctor => ctor.GetParameters().All(p => sp.CanResolve(p.ParameterType) || ParameterFill.CanResolve(p)));
            var maxFill = ctors.Max(ctor => ctor.GetParameters().Count());

            return ctors.Where(ctor => ctor.GetParameters().Count() == maxFill).Single();
        }
    }
}
