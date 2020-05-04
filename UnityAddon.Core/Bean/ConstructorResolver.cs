using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using UnityAddon.Core.Attributes;
using UnityAddon.Core.Bean.DependencyInjection;
using UnityAddon.Core.Exceptions;

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

            if (ctors.Count() == 0)
            {
                throw new BeanCreationException($"Fail to satisfy any of these constructors{string.Join("", type.GetConstructors().Select(ctor => "\r\n- " + ctor.ToString()))}");
            }

            var maxFill = ctors.Max(ctor => ctor.GetParameters().Count());
            var selectedCtors = ctors.Where(ctor => ctor.GetParameters().Count() == maxFill);

            if (selectedCtors.Count() > 1)
            {
                throw new BeanCreationException($"Ambiguous constructors are found{string.Join("", selectedCtors.Select(ctor => "\r\n- " + ctor.ToString()))}");
            }

            return selectedCtors.Single();
        }
    }
}
