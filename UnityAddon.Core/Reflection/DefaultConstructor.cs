using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnityAddon.Core.Reflection
{
    public static class DefaultConstructor
    {
        /// <summary>
        /// Choose the 1st constructor with InjectionConstructorAttribute, or
        /// the constructor with the max number of parameters
        /// </summary>
        public static ConstructorInfo Select(Type type)
        {
            var ctors = type.GetConstructors();
            var defaultCtor = ctors[0];

            for (var i = 1; i < ctors.Length && !defaultCtor.HasAttribute<InjectionConstructorAttribute>(); i++)
            {
                var ctor = ctors[i];

                if (ctor.GetParameters().Length > defaultCtor.GetParameters().Length)
                {
                    defaultCtor = ctor;
                }
            }

            return defaultCtor;
        }
    }
}
